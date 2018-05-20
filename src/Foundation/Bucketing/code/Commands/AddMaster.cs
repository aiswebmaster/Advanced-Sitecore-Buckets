using Sitecore;
using Sitecore.Buckets.Managers;
using Sitecore.Configuration;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMaster.Foundation.Bucketing.Commands
{
    public class AddMaster : Command
    {
        protected event ItemCreatedDelegate ItemCreated;
        public AddMaster()
        {
            this.ItemCreated += new ItemCreatedDelegate(this.OnItemCreated);
        }

        private void OnItemCreated(object sender, ItemCreatedEventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object)args, "args");
            Item obj = args.Item;
            if (obj == null || !BucketManager.IsItemContainedWithinBucket(obj))
                return;
            Context.ClientPage.ClientResponse.Eval("request.currentCommand = Number.MAX_VALUE;scForm.invoke(\"item:load(id=" + (object)obj.ID + ")\")");
        }

        public override void Execute(CommandContext context)
        {
            if (context.Items.Length != 1 || !context.Items[0].Access.CanCreate())
                return;

            Item contextItem = context.Items[0];
            NameValueCollection parameters = new NameValueCollection();

            parameters["Master"] = context.Parameters["master"];
            parameters["ItemID"] = contextItem.ID.ToString();
            parameters["Language"] = contextItem.Language.ToString();
            parameters["Version"] = contextItem.Version.ToString();

            Context.ClientPage.Start((object)this, "Add", parameters);
        }

        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject((object)context, "context");
            if (context.Items.Length != 1)
                return CommandState.Hidden;
            if (!context.Items[0].Access.CanCreate())
                return CommandState.Disabled;
            return base.QueryState(context);
        }

        protected void Add(ClientPipelineArgs args)
        {
            if (!SheerResponse.CheckModified())
                return;

            Item masterItem = Context.ContentDatabase.GetItem(args.Parameters["Master"]);

            if (masterItem == null)
                SheerResponse.Alert(Translate.Text("Branch \"{0}\" not found.", (object)args.Parameters["Master"]));

            else if (masterItem.TemplateID == TemplateIDs.CommandMaster)
            {
                string message = masterItem["Command"];
                if (string.IsNullOrEmpty(message))
                    return;

                Context.ClientPage.SendMessage((object)this, message);
            }
            else if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;

                Item parent = Context.ContentDatabase.Items[StringUtil.GetString(args.Parameters["ItemID"]), Language.Parse(StringUtil.GetString(args.Parameters["Language"]))];

                if (parent == null)
                    SheerResponse.Alert("Parent item not found.");

                else if (!parent.Access.CanCreate())
                {
                    Context.ClientPage.ClientResponse.Alert("You do not have permission to create items here");
                } else
                {
                    try
                    {
                        if (masterItem.TemplateID == TemplateIDs.BranchTemplate)
                        {
                            BranchItem branch = (BranchItem)masterItem;
                            Context.Workflow.AddItem(args.Result, branch, parent);
                        } else
                        {
                            TemplateItem template = (TemplateItem)masterItem;
                            Context.Workflow.AddItem(args.Result, template, parent);
                        }
                    } catch (WorkflowException ex)
                    {
                        Log.Error("Workflow error: could not add item from master", (Exception)ex, (object)this);
                        SheerResponse.Alert(ex.Message);
                    }
                }
            } else
            {
                if (BucketManager.IsBucket(masterItem))
                {
                    var url = new UrlString(UIUtil.GetUri("control:AddMasterDialog"));

                    url["id"] = args.Parameters["ItemID"];
                    url["lang"] = args.Parameters["Language"];
                    url["master"] = args.Parameters["Master"];
                    url["version"] = args.Parameters["Version"];

                    SheerResponse.ShowModalDialog(url.ToString());
                } else
                {
                    SheerResponse.Input("Enter a name for the new item***:", masterItem.DisplayName, Settings.ItemNameValidation, "'$Input' is not a valid name.", Settings.MaxItemNameLength);
                }

                args.WaitForPostBack();
            }
        }

    }
}
