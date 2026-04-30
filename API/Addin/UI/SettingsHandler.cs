using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using System;

namespace AxionFrame
{
    public sealed class SettingsHandler : IPropertyManagerPage2Handler9
    {
        private readonly SettingsPage _page;

        public SettingsHandler(SwAddin addin, SettingsPage page)
        {
            if (addin == null)
            {
                throw new ArgumentNullException(nameof(addin));
            }

            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            _page = page;
        }

        public void AfterClose()
        {
            int indentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(indentSize);
        }

        public void OnCheckboxCheck(int id, bool status)
        {
        }

        public void OnClose(int reason)
        {
            int indentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(indentSize);
        }

        public void OnComboboxEditChanged(int id, string text)
        {
        }

        public int OnActiveXControlCreated(int id, bool status)
        {
            return -1;
        }

        public void OnButtonPress(int id)
        {
        }

        public void OnComboboxSelectionChanged(int id, int item)
        {
        }

        public void OnGroupCheck(int id, bool status)
        {
        }

        public void OnGroupExpand(int id, bool status)
        {
        }

        public bool OnHelp()
        {
            string helpPath = "http://help.solidworks.com/2016/English/api/sldworksapiprogguide/Welcome.htm";
            System.Windows.Forms.Form helpForm = new System.Windows.Forms.Form();
            System.Windows.Forms.Help.ShowHelp(helpForm, helpPath);
            return true;
        }

        public void OnListboxSelectionChanged(int id, int item)
        {
        }

        public bool OnNextPage()
        {
            return true;
        }

        public void OnNumberboxChanged(int id, double val)
        {
        }

        public void OnNumberBoxTrackingCompleted(int id, double val)
        {
        }

        public void OnOptionCheck(int id)
        {
        }

        public bool OnPreviousPage()
        {
            return true;
        }

        public void OnSelectionboxCalloutCreated(int id)
        {
        }

        public void OnSelectionboxCalloutDestroyed(int id)
        {
        }

        public void OnSelectionboxFocusChanged(int id)
        {
        }

        public void OnSelectionboxListChanged(int id, int item)
        {
            if (_page.swPropertyPage != null)
            {
                _page.swPropertyPage.SetCursor(0);
            }
        }

        public void OnTextboxChanged(int id, string text)
        {
        }

        public void AfterActivation()
        {
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return true;
        }

        public void OnPopupMenuItem(int Id)
        {
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
        }

        public bool OnPreview()
        {
            return true;
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            return true;
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        public void OnUndo()
        {
        }

        public void OnWhatsNew()
        {
        }

        public void OnGainedFocus(int Id)
        {
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
        }

        public void OnLostFocus(int Id)
        {
        }

        public void OnRedo()
        {
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 0;
        }
    }
}
