using System.ComponentModel.Design;
using System.Web.UI.Design;

namespace Brandcaptcha.Design
{
    /// <summary>
    /// This class provide designer support code for <see cref="BrandcaptchaControl"/>.
    /// </summary>
    public class BrandcaptchaControlDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            return CreatePlaceHolderDesignTimeHtml("Brandcaptcha Validator");
        }

        public override bool AllowResize
        {
            get
            {
                return false;
            }
        }

        // Return a custom ActionList collection
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection _actionLists = new DesignerActionListCollection();
                _actionLists.AddRange(base.ActionLists);

                // Add a custom DesignerActionList
                _actionLists.Add(new ActionList(this));
                return _actionLists;
            }
        }

        public class ActionList : DesignerActionList
        {
            private BrandcaptchaControlDesigner _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionList"/> class.
            /// </summary>
            public ActionList(BrandcaptchaControlDesigner parent)
                : base(parent.Component)
            {
                this._parent = parent;
            }

            // Create the ActionItem collection and add one command
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                // fixme -- I can't get this to open up automatically (
                DesignerActionItemCollection items = new DesignerActionItemCollection();
                items.Add(new DesignerActionHeaderItem("API Key"));
                items.Add(new DesignerActionTextItem("To use Brandcaptcha, you need an API key", string.Empty));

                return items;
            }
        }
    }
}
