using SolidWorksTools.File;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AxionFrame
{
    public partial class SwAddin
    {
        #region UI Methods
        public void AddCommandMgr()
        {
            ICommandGroup cmdGroup;
            if (iBmp == null)
                iBmp = new BitmapHandler();
            Assembly thisAssembly;
            int cmdIndexSettings, cmdIndexBuild, cmdIndexOutput;
            string Title = "AxionFrame", ToolTip = "AxionFrame";


            int[] docTypes = new int[]{(int)swDocumentTypes_e.swDocASSEMBLY,
                                       (int)swDocumentTypes_e.swDocDRAWING,
                                       (int)swDocumentTypes_e.swDocPART};

            thisAssembly = System.Reflection.Assembly.GetAssembly(this.GetType());


            int cmdGroupErr = 0;
            bool ignorePrevious = false;

            object registryIDs;
            //get the ID information stored in the registry
            bool getDataResult = iCmdMgr.GetGroupDataFromRegistry(mainCmdGroupID, out registryIDs);

            int[] knownIDs = new int[3] { mainItemID1, mainItemID2, mainItemID3 };

            if (getDataResult)
            {
                if (!CompareIDs((int[])registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }

            cmdGroup = iCmdMgr.CreateCommandGroup2(mainCmdGroupID, Title, ToolTip, "", -1, ignorePrevious, ref cmdGroupErr);

            // Add bitmaps to your project and set them as embedded resources or provide a direct path to the bitmaps.
            icons[0] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar20x.png", thisAssembly);
            icons[1] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar32x.png", thisAssembly);
            icons[2] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar40x.png", thisAssembly);
            icons[3] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar64x.png", thisAssembly);
            icons[4] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar96x.png", thisAssembly);
            icons[5] = iBmp.CreateFileFromResourceBitmap("AxionFrame.toolbar128x.png", thisAssembly);

            mainIcons[0] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_20.png", thisAssembly);
            mainIcons[1] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_32.png", thisAssembly);
            mainIcons[2] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_40.png", thisAssembly);
            mainIcons[3] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_64.png", thisAssembly);
            mainIcons[4] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_96.png", thisAssembly);
            mainIcons[5] = iBmp.CreateFileFromResourceBitmap("AxionFrame.mainicon_128.png", thisAssembly);

            cmdGroup.MainIconList = mainIcons;
            cmdGroup.IconList = icons;

            int menuToolbarOption = (int)(swCommandItemType_e.swMenuItem | swCommandItemType_e.swToolbarItem);
            cmdIndexSettings = cmdGroup.AddCommandItem2("Settings", -1, "Display parameter settings", "Settings", 2, "ShowSettings", "EnableSettings", mainItemID1, menuToolbarOption);
            cmdIndexBuild = cmdGroup.AddCommandItem2("Build", -1, "Run Build workflow", "Build", 0, "RunBuildCommand", "EnableBuildCommand", mainItemID2, menuToolbarOption);
            cmdIndexOutput = cmdGroup.AddCommandItem2("Output", -1, "Run Final Output workflow", "Output", 1, "RunOutputCommand", "EnableOutputCommand", mainItemID3, menuToolbarOption);

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();

            bool bResult;

            FlyoutGroup flyGroup = iCmdMgr.CreateFlyoutGroup2(flyoutGroupID, "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
              cmdGroup.MainIconList, cmdGroup.IconList, "FlyoutCallback", "FlyoutEnable");

            flyGroup.AddCommandItem("FlyoutCommand 1", "test", 0, "FlyoutCommandItem1", "FlyoutEnableCommandItem1");

            flyGroup.FlyoutType = (int)swCommandFlyoutStyle_e.swCommandFlyoutStyle_Simple;

            foreach (int type in docTypes)
            {
                CommandTab cmdTab;

                cmdTab = iCmdMgr.GetCommandTab(type, Title);

                if (cmdTab != null && (!getDataResult || ignorePrevious))//if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
                {
                    bool res = iCmdMgr.RemoveCommandTab(cmdTab);
                    cmdTab = null;
                }

                //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
                if (cmdTab == null)
                {
                    cmdTab = iCmdMgr.AddCommandTab(type, Title);

                    CommandTabBox cmdBox = cmdTab.AddCommandTabBox();

                    int[] cmdIDs = new int[3];
                    int[] TextType = new int[3];

                    cmdIDs[0] = cmdGroup.get_CommandID(cmdIndexSettings);
                    TextType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;

                    cmdIDs[1] = cmdGroup.get_CommandID(cmdIndexBuild);
                    TextType[1] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;

                    cmdIDs[2] = cmdGroup.get_CommandID(cmdIndexOutput);
                    TextType[2] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;

                    bResult = cmdBox.AddCommands(cmdIDs, TextType);
                }
            }

            // Create a third-party icon in the context-sensitive menus of faces in parts
            // To see this menu, right click on any face in the part
            Frame swFrame;

            swFrame = iSwApp.Frame();
            bResult = swFrame.AddMenuPopupIcon3((int)swDocumentTypes_e.swDocPART, (int)swSelectType_e.swSelFACES, "third-party context-sensitive CSharp", addinID,
                                                "PopupCallbackFunction", "PopupEnable", "", cmdGroup.MainIconList);

            // create and register the shortcut menu
            registerID = iSwApp.RegisterThirdPartyPopupMenu();

            // add a menu break at the top of the shortcut menu
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Menu Break", addinID, "", "", "", "", "", (int)swMenuItemType_e.swMenuItemType_Break);

            // add a couple of items to the shortcut menu
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test1", addinID, "TestCallback", "EnableTest", "", "Test1", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test2", addinID, "TestCallback", "EnableTest", "", "Test2", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);

            // add a separator bar to the shortcut menu
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "separator", addinID, "", "", "", "", "", (int)swMenuItemType_e.swMenuItemType_Separator);

            // add another item to the shortcut menu
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test3", addinID, "TestCallback", "EnableTest", "", "Test3", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);

            // add an icon to a menu bar of the shortcut menu
            bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "", addinID, "TestCallback", "EnableTest", "", "NoOp", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);

            thisAssembly = null;
        }

        public void RemoveCommandMgr()
        {
            if (iBmp != null)
            {
                iBmp.Dispose();
                iBmp = null;
            }

            if (iCmdMgr != null)
            {
                iCmdMgr.RemoveCommandGroup(mainCmdGroupID);
                iCmdMgr.RemoveFlyoutGroup(flyoutGroupID);
            }
        }

        public bool CompareIDs(int[] storedIDs, int[] addinIDs)
        {
            if (storedIDs == null || addinIDs == null)
            {
                return false;
            }

            List<int> storedList = new List<int>(storedIDs);
            List<int> addinList = new List<int>(addinIDs);

            addinList.Sort();
            storedList.Sort();

            if (addinList.Count != storedList.Count)
            {
                return false;
            }
            else
            {

                for (int i = 0; i < addinList.Count; i++)
                {
                    if (addinList[i] != storedList[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Boolean AddSettings()
        {
            ppage = new SettingsPage(this);
            return true;
        }

        public Boolean RemoveSettings()
        {
            ppage = null;
            return true;
        }

        #endregion

        #region UI Callbacks
        public void RunBuildCommand()
        {
            if (iSwApp == null)
            {
                return;
            }

            try
            {
                DeterministicNamingService naming = new DeterministicNamingService();
                FrameModule frameModule = new FrameModule(naming, new SolidWorksFrameGeometryExecutor(iSwApp));
                BuildWorkflowEngine workflowEngine = new BuildWorkflowEngine(
                    new FeatureManager(),
                    frameModule,
                    new PivotModule(naming),
                    new HeightAdjustModule(naming),
                    new PlateBraceModule(naming));
                BuildExecutionResult result = workflowEngine.ExecuteBuild(null, null);

                int icon = result.IsSuccessful ? (int)swMessageBoxIcon_e.swMbInformation : (int)swMessageBoxIcon_e.swMbStop;
                iSwApp.SendMsgToUser2(result.ToDisplaySummary(), icon, (int)swMessageBoxBtn_e.swMbOk);
            }
            catch (Exception ex)
            {
                // Surface failures to the user as a controlled SolidWorks message instead of bubbling exceptions into COM callbacks.
                string message = "Build workflow failed unexpectedly." + System.Environment.NewLine +
                                 ex.GetType().Name + ": " + ex.Message;
                iSwApp.SendMsgToUser2(message, (int)swMessageBoxIcon_e.swMbStop, (int)swMessageBoxBtn_e.swMbOk);
            }
        }

        public int EnableBuildCommand()
        {
            if (iSwApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }

        public void RunOutputCommand()
        {
            if (iSwApp == null)
            {
                return;
            }

            string message = "Final Output workflow is not available yet." + System.Environment.NewLine +
                             "Use Build to generate the current CAD baseline.";
            iSwApp.SendMsgToUser2(message, (int)swMessageBoxIcon_e.swMbInformation, (int)swMessageBoxBtn_e.swMbOk);
        }

        public int EnableOutputCommand()
        {
            if (iSwApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }

        public void PopupCallbackFunction()
        {
            bool bRet;

            bRet = iSwApp.ShowThirdPartyPopupMenu(registerID, 500, 500);
        }

        public int PopupEnable()
        {
            if (iSwApp.ActiveDoc == null)
                return 0;
            else
                return 1;
        }

        public void TestCallback()
        {
            Debug.Print("Test Callback, CSharp");
        }

        public int EnableTest()
        {
            if (iSwApp.ActiveDoc == null)
                return 0;
            else
                return 1;
        }

        public void ShowSettings()
        {
            if (ppage != null)
                ppage.Show();
        }

        public int EnableSettings()
        {
            if (iSwApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }

        public void FlyoutCallback()
        {
            FlyoutGroup flyGroup = iCmdMgr.GetFlyoutGroup(flyoutGroupID);
            flyGroup.RemoveAllCommandItems();

            flyGroup.AddCommandItem(System.DateTime.Now.ToLongTimeString(), "test", 0, "FlyoutCommandItem1", "FlyoutEnableCommandItem1");

        }

        public int FlyoutEnable()
        {
            return 1;
        }

        public void FlyoutCommandItem1()
        {
            iSwApp.SendMsgToUser("Flyout command 1");
        }

        public int FlyoutEnableCommandItem1()
        {
            return 1;
        }
        #endregion
    }
}
