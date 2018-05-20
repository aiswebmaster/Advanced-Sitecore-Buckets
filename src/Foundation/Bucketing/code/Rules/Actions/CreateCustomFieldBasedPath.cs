using Sitecore;
using Sitecore.Buckets.Rules.Bucketing;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Rules.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMaster.Foundation.Bucketing.Rules.Actions
{
    public class CreateCustomFieldBasedPath<T> : RuleAction<T> where T : BucketingRuleContext
    {
        public string Field { get; set; }
        public string Format { get; set; }
        public override void Apply(T ruleContext)
        {
            ID fieldId = null;
            string value = string.Empty;
            Item currentItem = ruleContext.Database.GetItem(ruleContext.NewItemId);
            var pathParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(ruleContext.ResolvedPath))
            {
                pathParts.Add(ruleContext.ResolvedPath);
            }

            if (currentItem == null || string.IsNullOrWhiteSpace(Field))
                return;

            fieldId = ID.Parse(Field);

            if (fieldId.IsNull || currentItem.Fields[fieldId] == null || string.IsNullOrWhiteSpace(currentItem.Fields[fieldId].Value))
                return;

            if (FieldTypeManager.GetField(currentItem.Fields[fieldId]) is TextField)
            {
                value = currentItem[fieldId];
            }
            else if (FieldTypeManager.GetField(currentItem.Fields[fieldId]) is DateField)
            {
                DateField date = (DateField)currentItem.Fields[fieldId];
                value = date.DateTime.ToString(Format, Context.Culture).ToLowerInvariant();
            }

            pathParts.Insert(0, value);

            if (pathParts.Count > 0)
            {
                ruleContext.ResolvedPath = String.Join(Sitecore.Buckets.Util.Constants.ContentPathSeperator, pathParts.ToArray()).ToLowerInvariant();
            }
        }
    }
}
