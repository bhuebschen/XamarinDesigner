namespace SampleDesignerApplication
{
    using SampleDesignerHost;
    using System;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    public class KeystrokeMessageFilter : IMessageFilter
    {
        private IDesignerHost host;

        public KeystrokeMessageFilter(IDesignerHost host)
        {
            this.host = host;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if ((m.Msg == 0x100) && ((SampleDesignerHost) this.host).View.Focused)
            {
                IMenuCommandService mcs = this.host.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
                switch ((((int)m.WParam) | (int)Control.ModifierKeys))
                {
                    case 0x25:
                        mcs.GlobalInvoke(MenuCommands.KeyMoveLeft);
                        break;

                    case 0x26:
                        mcs.GlobalInvoke(MenuCommands.KeyMoveUp);
                        break;

                    case 0x27:
                        mcs.GlobalInvoke(MenuCommands.KeyMoveRight);
                        break;

                    case 40:
                        mcs.GlobalInvoke(MenuCommands.KeyMoveDown);
                        break;

                    case 0x1001b:
                        mcs.GlobalInvoke(MenuCommands.KeyReverseCancel);
                        break;

                    case 13:
                        mcs.GlobalInvoke(MenuCommands.KeyDefaultAction);
                        break;

                    case 0x1b:
                        mcs.GlobalInvoke(MenuCommands.KeyCancel);
                        break;

                    case 0x10025:
                        mcs.GlobalInvoke(MenuCommands.KeySizeWidthDecrease);
                        break;

                    case 0x10026:
                        mcs.GlobalInvoke(MenuCommands.KeySizeHeightIncrease);
                        break;

                    case 0x10027:
                        mcs.GlobalInvoke(MenuCommands.KeySizeWidthIncrease);
                        break;

                    case 0x10028:
                        mcs.GlobalInvoke(MenuCommands.KeySizeHeightDecrease);
                        break;

                    case 0x10035:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeWidthDecrease);
                        break;

                    case 0x20025:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeLeft);
                        break;

                    case 0x20026:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeUp);
                        break;

                    case 0x20027:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeRight);
                        break;

                    case 0x20028:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeDown);
                        break;

                    case 0x30026:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeHeightIncrease);
                        break;

                    case 0x30027:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeWidthIncrease);
                        break;

                    case 0x30028:
                        mcs.GlobalInvoke(MenuCommands.KeyNudgeHeightDecrease);
                        break;
                }
            }
            return false;
        }
    }
}

