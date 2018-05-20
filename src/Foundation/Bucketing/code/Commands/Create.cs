using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMaster.Foundation.Bucketing.Commands
{
    public class Create : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length != 1)
                return;

            Item obj = context.Items[0];

            if (!obj.Access.CanCreate())
            {
                SheerResponse.Alert("You do not have permission to create a new item here.");
            }
            else
            {
                string template = StringUtil.GetString(context.Parameters["template"]);
                string master = StringUtil.GetString(context.Parameters["master"]);
                string prompt = StringUtil.GetString(context.Parameters["prompt"]);
                NameValueCollection parameters = new NameValueCollection();

                BranchItem branchItem = (BranchItem)null;
                TemplateItem templateItem = (TemplateItem)null;

                if (master.Length > 0)
                {
                    branchItem = Context.ContentDatabase.Branches[master];
                    Error.Assert(branchItem != null, "Master \"" + master + "\" not found.");
                }
                else if (template.Length > 0)
                {
                    templateItem = Context.ContentDatabase.Templates[template];
                    Error.Assert(templateItem != null, "Template \"" + template + "\" not found.");
                }
                if (branchItem == null && templateItem == null)
                    return;
                parameters["prompt"] = prompt;
                parameters["id"] = obj.ID.ToString();
                parameters["database"] = obj.Database.Name;

                if (branchItem != null)
                    parameters["master"] = branchItem.ID.ToString();

                if (templateItem != null)
                    parameters["template"] = templateItem.ID.ToString();

                Context.ClientPage.Start((object)this, "Run", parameters);
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length != 1)
                return CommandState.Hidden;
            if (!context.Items[0].Access.CanDelete())
                return CommandState.Disabled;
            return base.QueryState(context);
        }

        protected void Run(ClientPipelineArgs args)
        {
            string master = StringUtil.GetString(args.Parameters["master"]);
            string template = StringUtil.GetString(args.Parameters["template"]);
            Database database = Factory.GetDatabase(StringUtil.GetString(args.Parameters["database"]));

            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;

                Item parent = database.Items[args.Parameters["id"]];
                if (parent != null)
                {
                    if (master.Length > 0)
                    {
                        BranchItem branch = database.Branches[master];
                        Log.Audit((object)this, "Add item : {0}", AuditFormatter.FormatItem(Context.Workflow.AddItem(args.Result, branch, parent)));
                    }
                    else
                    {
                        Log.Audit((object)this, "Add item : {0}", AuditFormatter.FormatItem(database.Templates[template].AddTo(parent, args.Result)));
                    }
                }
                else
                {
                    Context.ClientPage.ClientResponse.ShowError("Parent item not found.", "");
                    args.AbortPipeline();
                }
            }
            else
            {
                string str = string.Empty;
                Context.ClientPage.ClientResponse.Input("Enter a name for the new item ***:", master.Length <= 0 ? database.Templates[template].Name : database.Branches[master].Name, Settings.ItemNameValidation, "'$Input' is not a valid name.", Settings.MaxItemNameLength);
                args.WaitForPostBack();
            }
        }
    }
}