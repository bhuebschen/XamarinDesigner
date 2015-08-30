namespace SampleDesignerHost
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;

    internal class SampleDesignerTransaction : DesignerTransaction
    {
        private SampleDesignerHost host;

        public SampleDesignerTransaction(SampleDesignerHost host, string description) : base(description)
        {
            int cObject0000;
            this.host = host;
            host.TransactionDescriptions.Push(description);
            host.TransactionCount = (cObject0000 = host.TransactionCount) + 1;
            if (cObject0000 == 0)
            {
                host.OnTransactionOpening(EventArgs.Empty);
                host.OnTransactionOpened(EventArgs.Empty);
            }
        }

        protected override void OnCancel()
        {
            if (this.host != null)
            {
                Debug.Assert(this.host.TransactionDescriptions != null, "End batch operation with no desription?!?");
                string s = (string) this.host.TransactionDescriptions.Pop();
                if (--this.host.TransactionCount == 0)
                {
                    DesignerTransactionCloseEventArgs dtc = new DesignerTransactionCloseEventArgs(false);
                    this.host.OnTransactionClosing(dtc);
                    this.host.OnTransactionClosed(dtc);
                }
                this.host = null;
            }
        }

        protected override void OnCommit()
        {
            if (this.host != null)
            {
                Debug.Assert(this.host.TransactionDescriptions != null, "End batch operation with no desription?!?");
                string s = (string) this.host.TransactionDescriptions.Pop();
                if (--this.host.TransactionCount == 0)
                {
                    DesignerTransactionCloseEventArgs dtc = new DesignerTransactionCloseEventArgs(true);
                    this.host.OnTransactionClosing(dtc);
                    this.host.OnTransactionClosed(dtc);
                }
                this.host = null;
            }
        }
    }
}

