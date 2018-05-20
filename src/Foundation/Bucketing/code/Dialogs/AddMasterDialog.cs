using Helix.Feature.AdvancedBuckets.Infrastructure.Buckets;
using Sitecore;
using Sitecore.Buckets.Rules.Bucketing;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SitecoreMaster.Foundation.Bucketing.Dialogs
{
    public class AddMasterDialog : DialogForm
    {

        #region Fields

        protected Edit txtName;
        protected Label lblName;
        protected GridPanel gpCustomLabel;
        protected GridPanel gpCustomInput;

        #endregion

        #region Event Handlers

        protected override void OnOK(object sender, EventArgs args)
        {
            base.OnOK(sender, args);
        }

        protected override void OnCancel(object sender, EventArgs args)
        {
            base.OnCancel(sender, args);
            Context.ClientPage.ClientResponse.CloseWindow();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Context.ClientPage.IsPostBack)
            {
                var itemId = WebUtil.GetQueryString("id", string.Empty);
                var masterId = WebUtil.GetQueryString("master", string.Empty);
                Item parentItem = null;

                if (!string.IsNullOrEmpty(itemId) && ID.IsID(itemId))
                {
                    parentItem = Context.ContentDatabase.GetItem(new ID(itemId));
                }

                if (parentItem == null)
                    return;

                var bucketRulesItem = Context.ContentDatabase.GetItem(Sitecore.Buckets.Util.Constants.SettingsItemId);

                if (bucketRulesItem == null)
                    return;

                XDocument doc = XDocument.Parse(this.Rules);

                //BucketingRuleContext currentRuleContext = new BucketingRuleContext(Context.ContentDatabase, parentItem.ID, new ID(), "testing", new ID(masterId), DateTime.Now);

                //MatchedRule<BucketingRuleContext> matchedRule = bucketRulesItem.MatchRules<BucketingRuleContext>(Sitecore.Buckets.Util.Constants.BucketRulesFieldId, currentRuleContext);
                //RuleList<BucketingRuleContext> ruleCollection = RuleFactory.GetRules<BucketingRuleContext>(bucketRulesItem.Fields[Sitecore.Buckets.Util.Constants.BucketRulesFieldId]);

                //if (matchedRule != null && matchedRule.Context != null && matchedRule.Rule != null)
                //{
                //    var parameters = matchedRule.Context.Parameters;

                //    var txtCustomInput = new Text() { ID = "txtCustomInput", Field = "Bla" };
                //    var lblCustomInput = new Label() { ID = "lblCustomInput", Value = "Input a new PublishDate" };

                //    gpCustomLabel.Controls.Add(lblCustomInput);
                //    gpCustomInput.Controls.Add(txtCustomInput);
                //}
            }
        }

        #endregion


    }
}
