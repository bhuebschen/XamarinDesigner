namespace SampleDesignerApplication
{
    using SampleDesignerHost;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    public class MainForm : Form
    {
        private IContainer components = null;
        private KeystrokeMessageFilter filter;
        private SampleDesignerHost host;
        private ServiceContainer hostingServiceContainer;
        private SampleDesignerLoader loader;
        private MainMenu mainMenu;
        private MenuItem menuItemAlign;
        private MenuItem menuItemBottoms;
        private MenuItem menuItemBTF;
        private MenuItem menuItemBuild;
        private MenuItem menuItemCenter;
        private MenuItem menuItemCenters;
        private MenuItem menuItemControl;
        private MenuItem menuItemControlH;
        private MenuItem menuItemControlW;
        private MenuItem menuItemCopy;
        private MenuItem menuItemCSource;
        private MenuItem menuItemCut;
        private MenuItem menuItemDebug;
        private MenuItem menuItemDelete;
        private MenuItem menuItemDesign;
        private MenuItem menuItemEdit;
        private MenuItem menuItemExit;
        private MenuItem menuItemFile;
        private MenuItem menuItemGrid;
        private MenuItem menuItemHoriz;
        private MenuItem menuItemLayout;
        private MenuItem menuItemLefts;
        private MenuItem menuItemMiddles;
        private MenuItem menuItemNew;
        private MenuItem menuItemOpen;
        private MenuItem menuItemPaste;
        private MenuItem menuItemProperties;
        private MenuItem menuItemRedo;
        private MenuItem menuItemRights;
        private MenuItem menuItemRule;
        private MenuItem menuItemRun;
        private MenuItem menuItemSave;
        private MenuItem menuItemSaveAs;
        private MenuItem menuItemSelectAll;
        private MenuItem menuItemServiceRequests;
        private MenuItem menuItemShowGrid;
        private MenuItem menuItemSizeTo;
        private MenuItem menuItemSnapToGrid;
        private MenuItem menuItemSTB;
        private MenuItem menuItemStop;
        private MenuItem menuItemTO;
        private MenuItem menuItemToGrid;
        private MenuItem menuItemTops;
        private MenuItem menuItemUndo;
        private MenuItem menuItemVBSource;
        private MenuItem menuItemVert;
        private MenuItem menuItemView;
        private MenuItem menuItemXML;
        private MenuItem menuItemZO;
        private OpenFileDialog openFileDialog;
        private Panel panelMain;
        private PropertyGrid propertyGrid;
        private ServiceRequests serviceRequests;
        private Splitter splitter1;
        private Splitter splitter2;
        private TabControl tabControl;
        private TabPage tabCS;
        private TabPage tabDesign;
        private TabPage tabVB;
        private TabPage tabXML;
        private TextBox textCS;
        private TextBox textVB;
        private TextBox textXML;
        private ToolboxPane toolbox;

        public MainForm()
        {
            this.InitializeComponent();
            this.hostingServiceContainer = new ServiceContainer();
            this.hostingServiceContainer.AddService(typeof(PropertyGrid), this.propertyGrid);
            this.hostingServiceContainer.AddService(typeof(ToolboxPane), this.toolbox);
            this.hostingServiceContainer.AddService(typeof(TabControl), this.tabControl);
        }

        private void CreateDesigner(SampleDesignerLoader loader)
        {
            this.host = new SampleDesignerHost(this.hostingServiceContainer);
            this.propertyGrid.Site = new SampleDesignerApplication.PropertyGridSite(this.host, this);
            this.propertyGrid.PropertyTabs.AddTabType(typeof(EventsTab));
            this.host.LoadDocument(loader);
            this.loader = loader;
            this.toolbox.Host = this.host;
            this.host.View.Dock = DockStyle.Fill;
            this.host.View.Visible = true;
            this.panelMain.Controls.Add(this.host.View);
            this.tabControl.Visible = true;
            this.menuItemSave.Enabled = true;
            this.menuItemSaveAs.Enabled = true;
            this.menuItemEdit.Enabled = true;
            this.menuItemView.Enabled = true;
            this.menuItemLayout.Enabled = true;
            this.menuItemDebug.Enabled = true;
            this.filter = new KeystrokeMessageFilter(this.host);
            Application.AddMessageFilter(this.filter);
            this.host.SelectionChanged += new EventHandler(host_SelectionChanged);
        }

        void host_SelectionChanged(object sender, Object e)
        {
            MessageBox.Show("");

        }

        private bool DestroyDesigner()
        {
            if (this.loader != null)
            {
                if (this.loader.PromptDispose())
                {
                    this.tabControl.Visible = false;
                    this.menuItemSave.Enabled = false;
                    this.menuItemSaveAs.Enabled = false;
                    this.menuItemEdit.Enabled = false;
                    this.menuItemView.Enabled = false;
                    this.menuItemLayout.Enabled = false;
                    this.menuItemDebug.Enabled = false;
                    this.panelMain.Controls.Clear();
                    Application.RemoveMessageFilter(this.filter);
                    this.filter = null;
                    this.propertyGrid.Site = null;
                    this.host.Dispose();
                    this.loader = null;
                    this.host = null;
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.host != null))
            {
                this.host.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemNew = new System.Windows.Forms.MenuItem();
            this.menuItemOpen = new System.Windows.Forms.MenuItem();
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItemSaveAs = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemEdit = new System.Windows.Forms.MenuItem();
            this.menuItemUndo = new System.Windows.Forms.MenuItem();
            this.menuItemRedo = new System.Windows.Forms.MenuItem();
            this.menuItemRule = new System.Windows.Forms.MenuItem();
            this.menuItemCut = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemPaste = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemSelectAll = new System.Windows.Forms.MenuItem();
            this.menuItemView = new System.Windows.Forms.MenuItem();
            this.menuItemServiceRequests = new System.Windows.Forms.MenuItem();
            this.menuItemDesign = new System.Windows.Forms.MenuItem();
            this.menuItemCSource = new System.Windows.Forms.MenuItem();
            this.menuItemVBSource = new System.Windows.Forms.MenuItem();
            this.menuItemXML = new System.Windows.Forms.MenuItem();
            this.menuItemProperties = new System.Windows.Forms.MenuItem();
            this.menuItemLayout = new System.Windows.Forms.MenuItem();
            this.menuItemShowGrid = new System.Windows.Forms.MenuItem();
            this.menuItemSnapToGrid = new System.Windows.Forms.MenuItem();
            this.menuItemAlign = new System.Windows.Forms.MenuItem();
            this.menuItemLefts = new System.Windows.Forms.MenuItem();
            this.menuItemRights = new System.Windows.Forms.MenuItem();
            this.menuItemTops = new System.Windows.Forms.MenuItem();
            this.menuItemBottoms = new System.Windows.Forms.MenuItem();
            this.menuItemMiddles = new System.Windows.Forms.MenuItem();
            this.menuItemCenters = new System.Windows.Forms.MenuItem();
            this.menuItemToGrid = new System.Windows.Forms.MenuItem();
            this.menuItemCenter = new System.Windows.Forms.MenuItem();
            this.menuItemHoriz = new System.Windows.Forms.MenuItem();
            this.menuItemVert = new System.Windows.Forms.MenuItem();
            this.menuItemSizeTo = new System.Windows.Forms.MenuItem();
            this.menuItemControl = new System.Windows.Forms.MenuItem();
            this.menuItemControlW = new System.Windows.Forms.MenuItem();
            this.menuItemControlH = new System.Windows.Forms.MenuItem();
            this.menuItemGrid = new System.Windows.Forms.MenuItem();
            this.menuItemZO = new System.Windows.Forms.MenuItem();
            this.menuItemBTF = new System.Windows.Forms.MenuItem();
            this.menuItemSTB = new System.Windows.Forms.MenuItem();
            this.menuItemTO = new System.Windows.Forms.MenuItem();
            this.menuItemDebug = new System.Windows.Forms.MenuItem();
            this.menuItemBuild = new System.Windows.Forms.MenuItem();
            this.menuItemRun = new System.Windows.Forms.MenuItem();
            this.menuItemStop = new System.Windows.Forms.MenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDesign = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolbox = new SampleDesignerApplication.ToolboxPane();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabCS = new System.Windows.Forms.TabPage();
            this.textCS = new System.Windows.Forms.TextBox();
            this.tabVB = new System.Windows.Forms.TabPage();
            this.textVB = new System.Windows.Forms.TextBox();
            this.tabXML = new System.Windows.Forms.TabPage();
            this.textXML = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabDesign.SuspendLayout();
            this.tabCS.SuspendLayout();
            this.tabVB.SuspendLayout();
            this.tabXML.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemEdit,
            this.menuItemView,
            this.menuItemLayout,
            this.menuItemDebug});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNew,
            this.menuItemOpen,
            this.menuItemSave,
            this.menuItemSaveAs,
            this.menuItemExit});
            this.menuItemFile.Text = "&File";
            // 
            // menuItemNew
            // 
            this.menuItemNew.Index = 0;
            this.menuItemNew.Text = "&New";
            this.menuItemNew.Click += new System.EventHandler(this.menuItemNew_Click);
            // 
            // menuItemOpen
            // 
            this.menuItemOpen.Index = 1;
            this.menuItemOpen.Text = "&Open...";
            this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.Enabled = false;
            this.menuItemSave.Index = 2;
            this.menuItemSave.Text = "&Save";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItemSaveAs
            // 
            this.menuItemSaveAs.Enabled = false;
            this.menuItemSaveAs.Index = 3;
            this.menuItemSaveAs.Text = "Save &As...";
            this.menuItemSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 4;
            this.menuItemExit.Text = "&Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Enabled = false;
            this.menuItemEdit.Index = 1;
            this.menuItemEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemUndo,
            this.menuItemRedo,
            this.menuItemRule,
            this.menuItemCut,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemDelete,
            this.menuItemSelectAll});
            this.menuItemEdit.Text = "&Edit";
            // 
            // menuItemUndo
            // 
            this.menuItemUndo.Enabled = false;
            this.menuItemUndo.Index = 0;
            this.menuItemUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.menuItemUndo.Text = "Undo";
            // 
            // menuItemRedo
            // 
            this.menuItemRedo.Enabled = false;
            this.menuItemRedo.Index = 1;
            this.menuItemRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this.menuItemRedo.Text = "Redo";
            // 
            // menuItemRule
            // 
            this.menuItemRule.Index = 2;
            this.menuItemRule.Text = "-";
            // 
            // menuItemCut
            // 
            this.menuItemCut.Enabled = false;
            this.menuItemCut.Index = 3;
            this.menuItemCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuItemCut.Text = "Cu&t";
            this.menuItemCut.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Enabled = false;
            this.menuItemCopy.Index = 4;
            this.menuItemCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuItemCopy.Text = "&Copy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.Enabled = false;
            this.menuItemPaste.Index = 5;
            this.menuItemPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuItemPaste.Text = "&Paste";
            this.menuItemPaste.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 6;
            this.menuItemDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.menuItemDelete.Text = "&Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemSelectAll
            // 
            this.menuItemSelectAll.Index = 7;
            this.menuItemSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.menuItemSelectAll.Text = "Select &All";
            this.menuItemSelectAll.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemView
            // 
            this.menuItemView.Enabled = false;
            this.menuItemView.Index = 2;
            this.menuItemView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemServiceRequests,
            this.menuItemDesign,
            this.menuItemCSource,
            this.menuItemVBSource,
            this.menuItemXML,
            this.menuItemProperties});
            this.menuItemView.Text = "&View";
            // 
            // menuItemServiceRequests
            // 
            this.menuItemServiceRequests.Index = 0;
            this.menuItemServiceRequests.Text = "&Service Requests";
            this.menuItemServiceRequests.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemDesign
            // 
            this.menuItemDesign.Index = 1;
            this.menuItemDesign.Text = "&Design";
            this.menuItemDesign.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemCSource
            // 
            this.menuItemCSource.Index = 2;
            this.menuItemCSource.Text = "&C# Source";
            this.menuItemCSource.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemVBSource
            // 
            this.menuItemVBSource.Index = 3;
            this.menuItemVBSource.Text = "&VB Source";
            this.menuItemVBSource.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemXML
            // 
            this.menuItemXML.Index = 4;
            this.menuItemXML.Text = "&XML";
            this.menuItemXML.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemProperties
            // 
            this.menuItemProperties.Index = 5;
            this.menuItemProperties.Text = "&Properties";
            this.menuItemProperties.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemLayout
            // 
            this.menuItemLayout.Enabled = false;
            this.menuItemLayout.Index = 3;
            this.menuItemLayout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemShowGrid,
            this.menuItemSnapToGrid,
            this.menuItemAlign,
            this.menuItemCenter,
            this.menuItemSizeTo,
            this.menuItemZO,
            this.menuItemTO});
            this.menuItemLayout.Text = "&Layout";
            // 
            // menuItemShowGrid
            // 
            this.menuItemShowGrid.Checked = true;
            this.menuItemShowGrid.Index = 0;
            this.menuItemShowGrid.Text = "Show &Grid";
            this.menuItemShowGrid.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemSnapToGrid
            // 
            this.menuItemSnapToGrid.Checked = true;
            this.menuItemSnapToGrid.Index = 1;
            this.menuItemSnapToGrid.Text = "S&nap to Grid";
            this.menuItemSnapToGrid.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemAlign
            // 
            this.menuItemAlign.Index = 2;
            this.menuItemAlign.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLefts,
            this.menuItemRights,
            this.menuItemTops,
            this.menuItemBottoms,
            this.menuItemMiddles,
            this.menuItemCenters,
            this.menuItemToGrid});
            this.menuItemAlign.Text = "&Align...";
            // 
            // menuItemLefts
            // 
            this.menuItemLefts.Index = 0;
            this.menuItemLefts.Text = "&Lefts";
            this.menuItemLefts.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemRights
            // 
            this.menuItemRights.Index = 1;
            this.menuItemRights.Text = "&Rights";
            this.menuItemRights.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemTops
            // 
            this.menuItemTops.Index = 2;
            this.menuItemTops.Text = "&Tops";
            this.menuItemTops.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemBottoms
            // 
            this.menuItemBottoms.Index = 3;
            this.menuItemBottoms.Text = "&Bottoms";
            this.menuItemBottoms.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemMiddles
            // 
            this.menuItemMiddles.Index = 4;
            this.menuItemMiddles.Text = "&Middles";
            this.menuItemMiddles.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemCenters
            // 
            this.menuItemCenters.Index = 5;
            this.menuItemCenters.Text = "&Centers";
            this.menuItemCenters.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemToGrid
            // 
            this.menuItemToGrid.Index = 6;
            this.menuItemToGrid.Text = "to &Grid";
            this.menuItemToGrid.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemCenter
            // 
            this.menuItemCenter.Index = 3;
            this.menuItemCenter.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemHoriz,
            this.menuItemVert});
            this.menuItemCenter.Text = "&Center...";
            // 
            // menuItemHoriz
            // 
            this.menuItemHoriz.Index = 0;
            this.menuItemHoriz.Text = "&Horizontally";
            this.menuItemHoriz.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemVert
            // 
            this.menuItemVert.Index = 1;
            this.menuItemVert.Text = "&Vertically";
            this.menuItemVert.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemSizeTo
            // 
            this.menuItemSizeTo.Index = 4;
            this.menuItemSizeTo.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemControl,
            this.menuItemControlW,
            this.menuItemControlH,
            this.menuItemGrid});
            this.menuItemSizeTo.Text = "&Size to...";
            // 
            // menuItemControl
            // 
            this.menuItemControl.Index = 0;
            this.menuItemControl.Text = "&Control";
            this.menuItemControl.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemControlW
            // 
            this.menuItemControlW.Index = 1;
            this.menuItemControlW.Text = "Control &Width";
            this.menuItemControlW.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemControlH
            // 
            this.menuItemControlH.Index = 2;
            this.menuItemControlH.Text = "Control &Height";
            this.menuItemControlH.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemGrid
            // 
            this.menuItemGrid.Index = 3;
            this.menuItemGrid.Text = "&Grid";
            this.menuItemGrid.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemZO
            // 
            this.menuItemZO.Index = 5;
            this.menuItemZO.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemBTF,
            this.menuItemSTB});
            this.menuItemZO.Text = "&Z Order";
            // 
            // menuItemBTF
            // 
            this.menuItemBTF.Index = 0;
            this.menuItemBTF.Text = "&Bring to Front";
            this.menuItemBTF.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemSTB
            // 
            this.menuItemSTB.Index = 1;
            this.menuItemSTB.Text = "&Send to Back";
            this.menuItemSTB.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemTO
            // 
            this.menuItemTO.Index = 6;
            this.menuItemTO.Text = "&Tab Order";
            this.menuItemTO.Click += new System.EventHandler(this.menuItem_Click);
            // 
            // menuItemDebug
            // 
            this.menuItemDebug.Enabled = false;
            this.menuItemDebug.Index = 4;
            this.menuItemDebug.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemBuild,
            this.menuItemRun,
            this.menuItemStop});
            this.menuItemDebug.Text = "&Debug";
            // 
            // menuItemBuild
            // 
            this.menuItemBuild.Index = 0;
            this.menuItemBuild.Text = "&Build...";
            this.menuItemBuild.Click += new System.EventHandler(this.menuItemBuild_Click);
            // 
            // menuItemRun
            // 
            this.menuItemRun.Index = 1;
            this.menuItemRun.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuItemRun.Text = "&Run";
            this.menuItemRun.Click += new System.EventHandler(this.menuItemRun_Click);
            // 
            // menuItemStop
            // 
            this.menuItemStop.Index = 2;
            this.menuItemStop.Text = "&Stop";
            this.menuItemStop.Click += new System.EventHandler(this.menuItemStop_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDesign);
            this.tabControl.Controls.Add(this.tabCS);
            this.tabControl.Controls.Add(this.tabVB);
            this.tabControl.Controls.Add(this.tabXML);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.ItemSize = new System.Drawing.Size(120, 25);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 257);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            this.tabControl.Visible = false;
            this.tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl_DrawItem);
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabDesign
            // 
            this.tabDesign.Controls.Add(this.splitter2);
            this.tabDesign.Controls.Add(this.panelMain);
            this.tabDesign.Controls.Add(this.splitter1);
            this.tabDesign.Controls.Add(this.toolbox);
            this.tabDesign.Controls.Add(this.propertyGrid);
            this.tabDesign.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabDesign.Location = new System.Drawing.Point(4, 29);
            this.tabDesign.Name = "tabDesign";
            this.tabDesign.Size = new System.Drawing.Size(752, 224);
            this.tabDesign.TabIndex = 0;
            this.tabDesign.Text = "Design";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(520, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 224);
            this.splitter2.TabIndex = 12;
            this.splitter2.TabStop = false;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelMain.Location = new System.Drawing.Point(291, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(232, 224);
            this.panelMain.TabIndex = 11;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(288, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 224);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // toolbox
            // 
            this.toolbox.BackColor = System.Drawing.Color.Black;
            this.toolbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolbox.Host = null;
            this.toolbox.Location = new System.Drawing.Point(0, 0);
            this.toolbox.Name = "toolbox";
            this.toolbox.Size = new System.Drawing.Size(288, 224);
            this.toolbox.TabIndex = 9;
            // 
            // propertyGrid
            // 
            this.propertyGrid.BackColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.CausesValidation = false;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGrid.LineColor = System.Drawing.Color.LightSlateGray;
            this.propertyGrid.Location = new System.Drawing.Point(523, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(229, 224);
            this.propertyGrid.TabIndex = 7;
            // 
            // tabCS
            // 
            this.tabCS.Controls.Add(this.textCS);
            this.tabCS.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabCS.Location = new System.Drawing.Point(4, 29);
            this.tabCS.Name = "tabCS";
            this.tabCS.Size = new System.Drawing.Size(752, 224);
            this.tabCS.TabIndex = 2;
            this.tabCS.Text = "C# Source";
            // 
            // textCS
            // 
            this.textCS.BackColor = System.Drawing.Color.DarkSlateGray;
            this.textCS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textCS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textCS.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textCS.ForeColor = System.Drawing.Color.Gold;
            this.textCS.Location = new System.Drawing.Point(0, 0);
            this.textCS.Multiline = true;
            this.textCS.Name = "textCS";
            this.textCS.ReadOnly = true;
            this.textCS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textCS.Size = new System.Drawing.Size(752, 224);
            this.textCS.TabIndex = 1;
            this.textCS.WordWrap = false;
            // 
            // tabVB
            // 
            this.tabVB.Controls.Add(this.textVB);
            this.tabVB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabVB.Location = new System.Drawing.Point(4, 29);
            this.tabVB.Name = "tabVB";
            this.tabVB.Size = new System.Drawing.Size(752, 224);
            this.tabVB.TabIndex = 1;
            this.tabVB.Text = "VB Source";
            // 
            // textVB
            // 
            this.textVB.BackColor = System.Drawing.Color.DarkSlateGray;
            this.textVB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textVB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textVB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textVB.ForeColor = System.Drawing.Color.Tomato;
            this.textVB.Location = new System.Drawing.Point(0, 0);
            this.textVB.Multiline = true;
            this.textVB.Name = "textVB";
            this.textVB.ReadOnly = true;
            this.textVB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textVB.Size = new System.Drawing.Size(752, 224);
            this.textVB.TabIndex = 0;
            this.textVB.WordWrap = false;
            // 
            // tabXML
            // 
            this.tabXML.Controls.Add(this.textXML);
            this.tabXML.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabXML.Location = new System.Drawing.Point(4, 29);
            this.tabXML.Name = "tabXML";
            this.tabXML.Size = new System.Drawing.Size(752, 224);
            this.tabXML.TabIndex = 3;
            this.tabXML.Text = "XML";
            // 
            // textXML
            // 
            this.textXML.BackColor = System.Drawing.Color.DarkSlateGray;
            this.textXML.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textXML.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textXML.ForeColor = System.Drawing.Color.LimeGreen;
            this.textXML.Location = new System.Drawing.Point(0, 0);
            this.textXML.Multiline = true;
            this.textXML.Name = "textXML";
            this.textXML.ReadOnly = true;
            this.textXML.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textXML.Size = new System.Drawing.Size(752, 224);
            this.textXML.TabIndex = 1;
            this.textXML.WordWrap = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "XamarinDesigner Files|*.xdx";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(760, 257);
            this.Controls.Add(this.tabControl);
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Simple Designer Host Sample";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl.ResumeLayout(false);
            this.tabDesign.ResumeLayout(false);
            this.tabCS.ResumeLayout(false);
            this.tabCS.PerformLayout();
            this.tabVB.ResumeLayout(false);
            this.tabVB.PerformLayout();
            this.tabXML.ResumeLayout(false);
            this.tabXML.PerformLayout();
            this.ResumeLayout(false);

        }

        [STAThread]
        public static void Main()
        {
            Application.Run(new MainForm());
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            IMenuCommandService mcs = ((IServiceProvider) this.host).GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            switch ((sender as MenuItem).Text)
            {
                case "Cu&t":
                    mcs.GlobalInvoke(StandardCommands.Cut);
                    break;

                case "&Copy":
                    mcs.GlobalInvoke(StandardCommands.Copy);
                    break;

                case "&Paste":
                    mcs.GlobalInvoke(StandardCommands.Paste);
                    break;

                case "&Delete":
                    mcs.GlobalInvoke(StandardCommands.Delete);
                    break;

                case "Select &All":
                    mcs.GlobalInvoke(StandardCommands.SelectAll);
                    break;

                case "&Service Requests":
                    if (this.serviceRequests == null)
                    {
                        this.serviceRequests = new ServiceRequests();
                        this.serviceRequests.Closed += new EventHandler(this.OnServiceRequestsClosed);
                        this.hostingServiceContainer.AddService(typeof(ServiceRequests), this.serviceRequests);
                        this.serviceRequests.Show();
                    }
                    this.serviceRequests.Activate();
                    break;

                case "&Design":
                    this.tabControl.SelectedTab = this.tabDesign;
                    break;

                case "&C# Source":
                    this.tabControl.SelectedTab = this.tabCS;
                    break;

                case "&VB Source":
                    this.tabControl.SelectedTab = this.tabVB;
                    break;

                case "&XML":
                    this.tabControl.SelectedTab = this.tabXML;
                    break;

                case "&Properties":
                    mcs.GlobalInvoke(StandardCommands.Properties);
                    break;

                case "Show &Grid":
                    mcs.GlobalInvoke(StandardCommands.ShowGrid);
                    this.menuItemShowGrid.Checked = !this.menuItemShowGrid.Checked;
                    break;

                case "S&nap to Grid":
                    mcs.GlobalInvoke(StandardCommands.SnapToGrid);
                    this.menuItemSnapToGrid.Checked = !this.menuItemSnapToGrid.Checked;
                    break;

                case "&Lefts":
                    mcs.GlobalInvoke(StandardCommands.AlignLeft);
                    break;

                case "&Rights":
                    mcs.GlobalInvoke(StandardCommands.AlignRight);
                    break;

                case "&Tops":
                    mcs.GlobalInvoke(StandardCommands.AlignTop);
                    break;

                case "&Bottoms":
                    mcs.GlobalInvoke(StandardCommands.AlignBottom);
                    break;

                case "&Middles":
                    mcs.GlobalInvoke(StandardCommands.AlignHorizontalCenters);
                    break;

                case "&Centers":
                    mcs.GlobalInvoke(StandardCommands.AlignVerticalCenters);
                    break;

                case "to &Grid":
                    mcs.GlobalInvoke(StandardCommands.AlignToGrid);
                    break;

                case "&Horizontally":
                    mcs.GlobalInvoke(StandardCommands.CenterHorizontally);
                    break;

                case "&Vertically":
                    mcs.GlobalInvoke(StandardCommands.CenterVertically);
                    break;

                case "&Control":
                    mcs.GlobalInvoke(StandardCommands.SizeToControl);
                    break;

                case "Control &Width":
                    mcs.GlobalInvoke(StandardCommands.SizeToControlWidth);
                    break;

                case "Control &Height":
                    mcs.GlobalInvoke(StandardCommands.SizeToControlHeight);
                    break;

                case "&Grid":
                    mcs.GlobalInvoke(StandardCommands.SizeToGrid);
                    break;

                case "&Bring to Front":
                    mcs.GlobalInvoke(StandardCommands.BringToFront);
                    break;

                case "&Send to Back":
                    mcs.GlobalInvoke(StandardCommands.SendToBack);
                    break;

                case "&Tab Order":
                    mcs.GlobalInvoke(StandardCommands.TabOrder);
                    break;
            }
        }

        private void menuItemBuild_Click(object sender, EventArgs e)
        {
            this.loader.Build();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            if (this.DestroyDesigner())
            {
                base.Close();
            }
        }

        private void menuItemNew_Click(object sender, EventArgs e)
        {
            if (this.DestroyDesigner())
            {
                SampleDesignerLoader designerLoader = new SampleDesignerLoader();
                this.CreateDesigner(designerLoader);
            }
        }

        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            if (this.DestroyDesigner() && (this.openFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                SampleDesignerLoader designerLoader = new SampleDesignerLoader(this.openFileDialog.FileName);
                this.CreateDesigner(designerLoader);
            }
        }

        private void menuItemRun_Click(object sender, EventArgs e)
        {
            this.loader.Run();
        }

        private void menuItemSave_Click(object sender, EventArgs e)
        {
            this.loader.Save(false);
        }

        private void menuItemSaveAs_Click(object sender, EventArgs e)
        {
            this.loader.Save(true);
        }

        private void menuItemStop_Click(object sender, EventArgs e)
        {
            this.loader.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.DestroyDesigner())
            {
                e.Cancel = false;
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
                base.OnClosing(e);
            }
        }

        private void OnServiceRequestsClosed(object sender, EventArgs e)
        {
            this.serviceRequests = null;
            this.hostingServiceContainer.RemoveService(typeof(ServiceRequests));
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tcSender = sender as TabControl;
            string text = tcSender.TabPages[e.Index].Text;
            Size textSize = e.Graphics.MeasureString(text, tcSender.Font).ToSize();
            switch (e.Index)
            {
                case 0:
                    e.Graphics.FillRectangle(Brushes.SteelBlue, e.Bounds);
                    break;

                case 1:
                    e.Graphics.FillRectangle(Brushes.Gold, e.Bounds);
                    break;

                case 2:
                    e.Graphics.FillRectangle(Brushes.Tomato, e.Bounds);
                    break;

                case 3:
                    e.Graphics.FillRectangle(Brushes.LimeGreen, e.Bounds);
                    break;
            }
            Point textLocation = new Point(e.Bounds.X + ((e.Bounds.Width - textSize.Width) / 2), e.Bounds.Y + ((e.Bounds.Height - textSize.Height) / 2));
            e.Graphics.DrawString(text, tcSender.Font, Brushes.White, (PointF) textLocation);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.loader.Flush();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
             
        }
    }
}

