
namespace Automation
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BtnRecord = new System.Windows.Forms.Button();
            this.ChkLoopKey = new System.Windows.Forms.CheckBox();
            this.NumericWaitTime = new System.Windows.Forms.NumericUpDown();
            this.NumericLoopCount = new System.Windows.Forms.NumericUpDown();
            this.LabelWaitTime = new System.Windows.Forms.Label();
            this.LabelMoveSpeed = new System.Windows.Forms.Label();
            this.LabelLoopCount = new System.Windows.Forms.Label();
            this.ListMoveSpeed = new System.Windows.Forms.ComboBox();
            this.ListActionType = new System.Windows.Forms.ComboBox();
            this.TxtNewAction = new System.Windows.Forms.TextBox();
            this.LabelOnKey = new System.Windows.Forms.Label();
            this.TxtOnKey = new System.Windows.Forms.TextBox();
            this.ColumnKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BtnStartStop = new System.Windows.Forms.Button();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.ColumnOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnLoop = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnWait = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ListMain = new System.Windows.Forms.ListView();
            this.ColumnMisc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContextListMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextListMainDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ChkClick = new System.Windows.Forms.CheckBox();
            this.BtnNewKey = new System.Windows.Forms.Button();
            this.LabelMoveType = new System.Windows.Forms.Label();
            this.BtnOnKey = new System.Windows.Forms.Button();
            this.BtnImageBrw = new System.Windows.Forms.Button();
            this.ListImageLoc = new System.Windows.Forms.ComboBox();
            this.LabelImageLoc = new System.Windows.Forms.Label();
            this.ListMoveType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumericWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericLoopCount)).BeginInit();
            this.ContextListMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnRecord
            // 
            this.BtnRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRecord.Location = new System.Drawing.Point(747, 214);
            this.BtnRecord.Name = "BtnRecord";
            this.BtnRecord.Size = new System.Drawing.Size(94, 23);
            this.BtnRecord.TabIndex = 45;
            this.BtnRecord.Text = "Record";
            this.BtnRecord.UseVisualStyleBackColor = true;
            this.BtnRecord.Click += new System.EventHandler(this.BtnRecord_Click);
            // 
            // ChkLoopKey
            // 
            this.ChkLoopKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkLoopKey.Appearance = System.Windows.Forms.Appearance.Button;
            this.ChkLoopKey.Location = new System.Drawing.Point(658, 214);
            this.ChkLoopKey.Name = "ChkLoopKey";
            this.ChkLoopKey.Size = new System.Drawing.Size(89, 23);
            this.ChkLoopKey.TabIndex = 42;
            this.ChkLoopKey.Text = "Loop Key";
            this.ChkLoopKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ChkLoopKey.UseVisualStyleBackColor = true;
            // 
            // NumericWaitTime
            // 
            this.NumericWaitTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericWaitTime.Location = new System.Drawing.Point(749, 111);
            this.NumericWaitTime.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NumericWaitTime.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumericWaitTime.Name = "NumericWaitTime";
            this.NumericWaitTime.Size = new System.Drawing.Size(91, 20);
            this.NumericWaitTime.TabIndex = 35;
            this.NumericWaitTime.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumericWaitTime.ValueChanged += new System.EventHandler(this.NumericWaitTime_ValueChanged);
            // 
            // NumericLoopCount
            // 
            this.NumericLoopCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericLoopCount.Location = new System.Drawing.Point(659, 111);
            this.NumericLoopCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NumericLoopCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericLoopCount.Name = "NumericLoopCount";
            this.NumericLoopCount.Size = new System.Drawing.Size(88, 20);
            this.NumericLoopCount.TabIndex = 33;
            this.NumericLoopCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericLoopCount.ValueChanged += new System.EventHandler(this.NumericLoopCount_ValueChanged);
            // 
            // LabelWaitTime
            // 
            this.LabelWaitTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelWaitTime.AutoSize = true;
            this.LabelWaitTime.Location = new System.Drawing.Point(748, 95);
            this.LabelWaitTime.Name = "LabelWaitTime";
            this.LabelWaitTime.Size = new System.Drawing.Size(77, 13);
            this.LabelWaitTime.TabIndex = 34;
            this.LabelWaitTime.Text = "Wait Time (ms)";
            // 
            // LabelMoveSpeed
            // 
            this.LabelMoveSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelMoveSpeed.AutoSize = true;
            this.LabelMoveSpeed.Location = new System.Drawing.Point(754, 139);
            this.LabelMoveSpeed.Name = "LabelMoveSpeed";
            this.LabelMoveSpeed.Size = new System.Drawing.Size(68, 13);
            this.LabelMoveSpeed.TabIndex = 38;
            this.LabelMoveSpeed.Text = "Move Speed";
            // 
            // LabelLoopCount
            // 
            this.LabelLoopCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelLoopCount.AutoSize = true;
            this.LabelLoopCount.Location = new System.Drawing.Point(660, 95);
            this.LabelLoopCount.Name = "LabelLoopCount";
            this.LabelLoopCount.Size = new System.Drawing.Size(62, 13);
            this.LabelLoopCount.TabIndex = 32;
            this.LabelLoopCount.Text = "Loop Count";
            // 
            // ListMoveSpeed
            // 
            this.ListMoveSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListMoveSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListMoveSpeed.FormattingEnabled = true;
            this.ListMoveSpeed.Items.AddRange(new object[] {
            "Instant",
            "Line slow",
            "Line fast",
            "Curve slow",
            "Curve fast"});
            this.ListMoveSpeed.Location = new System.Drawing.Point(754, 155);
            this.ListMoveSpeed.Name = "ListMoveSpeed";
            this.ListMoveSpeed.Size = new System.Drawing.Size(86, 21);
            this.ListMoveSpeed.TabIndex = 39;
            this.ListMoveSpeed.SelectedIndexChanged += new System.EventHandler(this.ListMoveSpeed_SelectedIndexChanged);
            // 
            // ListActionType
            // 
            this.ListActionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListActionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListActionType.FormattingEnabled = true;
            this.ListActionType.Items.AddRange(new object[] {
            "New Position (X Y)",
            "Key Press",
            "Image Search"});
            this.ListActionType.Location = new System.Drawing.Point(659, 45);
            this.ListActionType.Name = "ListActionType";
            this.ListActionType.Size = new System.Drawing.Size(126, 21);
            this.ListActionType.TabIndex = 27;
            this.ListActionType.SelectedIndexChanged += new System.EventHandler(this.ListActionType_SelectedIndexChanged);
            // 
            // TxtNewAction
            // 
            this.TxtNewAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtNewAction.Location = new System.Drawing.Point(659, 68);
            this.TxtNewAction.MaxLength = 11;
            this.TxtNewAction.Name = "TxtNewAction";
            this.TxtNewAction.Size = new System.Drawing.Size(181, 20);
            this.TxtNewAction.TabIndex = 31;
            this.TxtNewAction.Text = "0 0";
            this.TxtNewAction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNewAction_KeyDown);
            this.TxtNewAction.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNewAction_KeyPress);
            this.TxtNewAction.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtNewAction_KeyUp);
            // 
            // LabelOnKey
            // 
            this.LabelOnKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelOnKey.AutoSize = true;
            this.LabelOnKey.Location = new System.Drawing.Point(660, 5);
            this.LabelOnKey.Name = "LabelOnKey";
            this.LabelOnKey.Size = new System.Drawing.Size(45, 13);
            this.LabelOnKey.TabIndex = 24;
            this.LabelOnKey.Text = "On Key:";
            // 
            // TxtOnKey
            // 
            this.TxtOnKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOnKey.BackColor = System.Drawing.SystemColors.Window;
            this.TxtOnKey.Location = new System.Drawing.Point(659, 20);
            this.TxtOnKey.Name = "TxtOnKey";
            this.TxtOnKey.ReadOnly = true;
            this.TxtOnKey.Size = new System.Drawing.Size(181, 20);
            this.TxtOnKey.TabIndex = 26;
            // 
            // ColumnKey
            // 
            this.ColumnKey.Text = "Key";
            // 
            // BtnStartStop
            // 
            this.BtnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStartStop.Location = new System.Drawing.Point(658, 237);
            this.BtnStartStop.Name = "BtnStartStop";
            this.BtnStartStop.Size = new System.Drawing.Size(89, 23);
            this.BtnStartStop.TabIndex = 43;
            this.BtnStartStop.Text = "Stop";
            this.BtnStartStop.UseVisualStyleBackColor = true;
            this.BtnStartStop.Click += new System.EventHandler(this.BtnStartStop_Click);
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAdd.Location = new System.Drawing.Point(747, 237);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(94, 23);
            this.BtnAdd.TabIndex = 44;
            this.BtnAdd.Text = "Add";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // ColumnOrder
            // 
            this.ColumnOrder.Text = "Order";
            this.ColumnOrder.Width = 45;
            // 
            // ColumnAction
            // 
            this.ColumnAction.Text = "Action";
            this.ColumnAction.Width = 81;
            // 
            // ColumnValue
            // 
            this.ColumnValue.Text = "Value";
            this.ColumnValue.Width = 111;
            // 
            // ColumnLoop
            // 
            this.ColumnLoop.Text = "Loop";
            this.ColumnLoop.Width = 54;
            // 
            // ColumnWait
            // 
            this.ColumnWait.Text = "Wait";
            // 
            // ListMain
            // 
            this.ListMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnKey,
            this.ColumnOrder,
            this.ColumnAction,
            this.ColumnValue,
            this.ColumnLoop,
            this.ColumnWait,
            this.ColumnMisc});
            this.ListMain.ContextMenuStrip = this.ContextListMain;
            this.ListMain.FullRowSelect = true;
            this.ListMain.HideSelection = false;
            this.ListMain.Location = new System.Drawing.Point(2, 2);
            this.ListMain.Name = "ListMain";
            this.ListMain.Size = new System.Drawing.Size(655, 257);
            this.ListMain.TabIndex = 23;
            this.ListMain.UseCompatibleStateImageBehavior = false;
            this.ListMain.View = System.Windows.Forms.View.Details;
            // 
            // ColumnMisc
            // 
            this.ColumnMisc.Text = "Misc";
            this.ColumnMisc.Width = 209;
            // 
            // ContextListMain
            // 
            this.ContextListMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextListMainDelete});
            this.ContextListMain.Name = "ContextListMain";
            this.ContextListMain.Size = new System.Drawing.Size(108, 26);
            // 
            // ContextListMainDelete
            // 
            this.ContextListMainDelete.Name = "ContextListMainDelete";
            this.ContextListMainDelete.Size = new System.Drawing.Size(107, 22);
            this.ContextListMainDelete.Text = "Delete";
            this.ContextListMainDelete.Click += new System.EventHandler(this.ContextListMainDelete_Click);
            // 
            // ChkClick
            // 
            this.ChkClick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkClick.Appearance = System.Windows.Forms.Appearance.Button;
            this.ChkClick.Location = new System.Drawing.Point(786, 44);
            this.ChkClick.Name = "ChkClick";
            this.ChkClick.Size = new System.Drawing.Size(55, 23);
            this.ChkClick.TabIndex = 28;
            this.ChkClick.Text = "Click";
            this.ChkClick.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ChkClick.UseVisualStyleBackColor = true;
            this.ChkClick.CheckedChanged += new System.EventHandler(this.ChkClick_CheckedChanged);
            // 
            // BtnNewKey
            // 
            this.BtnNewKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnNewKey.Location = new System.Drawing.Point(786, 44);
            this.BtnNewKey.Name = "BtnNewKey";
            this.BtnNewKey.Size = new System.Drawing.Size(55, 23);
            this.BtnNewKey.TabIndex = 29;
            this.BtnNewKey.Text = "Set";
            this.BtnNewKey.UseVisualStyleBackColor = true;
            this.BtnNewKey.Visible = false;
            this.BtnNewKey.Click += new System.EventHandler(this.BtnNewKey_Click);
            // 
            // LabelMoveType
            // 
            this.LabelMoveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelMoveType.AutoSize = true;
            this.LabelMoveType.Location = new System.Drawing.Point(660, 139);
            this.LabelMoveType.Name = "LabelMoveType";
            this.LabelMoveType.Size = new System.Drawing.Size(61, 13);
            this.LabelMoveType.TabIndex = 36;
            this.LabelMoveType.Text = "Move Type";
            // 
            // BtnOnKey
            // 
            this.BtnOnKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOnKey.Location = new System.Drawing.Point(786, 0);
            this.BtnOnKey.Name = "BtnOnKey";
            this.BtnOnKey.Size = new System.Drawing.Size(55, 20);
            this.BtnOnKey.TabIndex = 25;
            this.BtnOnKey.Text = "Set";
            this.BtnOnKey.UseVisualStyleBackColor = true;
            this.BtnOnKey.Click += new System.EventHandler(this.BtnOnKey_Click);
            // 
            // BtnImageBrw
            // 
            this.BtnImageBrw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnImageBrw.Location = new System.Drawing.Point(786, 44);
            this.BtnImageBrw.Name = "BtnImageBrw";
            this.BtnImageBrw.Size = new System.Drawing.Size(55, 23);
            this.BtnImageBrw.TabIndex = 30;
            this.BtnImageBrw.Text = "...";
            this.BtnImageBrw.UseVisualStyleBackColor = true;
            this.BtnImageBrw.Visible = false;
            this.BtnImageBrw.Click += new System.EventHandler(this.BtnImageBrw_Click);
            // 
            // ListImageLoc
            // 
            this.ListImageLoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListImageLoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListImageLoc.FormattingEnabled = true;
            this.ListImageLoc.Items.AddRange(new object[] {
            "Left Top",
            "Left Middle",
            "Left Bottom",
            "Middle Top",
            "Middle",
            "Middle Bottom",
            "Right Top",
            "Right Middle",
            "Right Bottom"});
            this.ListImageLoc.Location = new System.Drawing.Point(659, 155);
            this.ListImageLoc.Name = "ListImageLoc";
            this.ListImageLoc.Size = new System.Drawing.Size(93, 21);
            this.ListImageLoc.TabIndex = 41;
            this.ListImageLoc.SelectedIndexChanged += new System.EventHandler(this.ListImageLoc_SelectedIndexChanged);
            // 
            // LabelImageLoc
            // 
            this.LabelImageLoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelImageLoc.AutoSize = true;
            this.LabelImageLoc.Location = new System.Drawing.Point(660, 139);
            this.LabelImageLoc.Name = "LabelImageLoc";
            this.LabelImageLoc.Size = new System.Drawing.Size(80, 13);
            this.LabelImageLoc.TabIndex = 40;
            this.LabelImageLoc.Text = "Image Location";
            // 
            // ListMoveType
            // 
            this.ListMoveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListMoveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListMoveType.FormattingEnabled = true;
            this.ListMoveType.Items.AddRange(new object[] {
            "Relative",
            "Absolute"});
            this.ListMoveType.Location = new System.Drawing.Point(659, 155);
            this.ListMoveType.Name = "ListMoveType";
            this.ListMoveType.Size = new System.Drawing.Size(93, 21);
            this.ListMoveType.TabIndex = 37;
            this.ListMoveType.SelectedIndexChanged += new System.EventHandler(this.ListMoveType_SelectedIndexChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 261);
            this.Controls.Add(this.BtnRecord);
            this.Controls.Add(this.ChkLoopKey);
            this.Controls.Add(this.NumericWaitTime);
            this.Controls.Add(this.NumericLoopCount);
            this.Controls.Add(this.LabelWaitTime);
            this.Controls.Add(this.LabelMoveSpeed);
            this.Controls.Add(this.LabelLoopCount);
            this.Controls.Add(this.ListMoveSpeed);
            this.Controls.Add(this.ListActionType);
            this.Controls.Add(this.TxtNewAction);
            this.Controls.Add(this.LabelOnKey);
            this.Controls.Add(this.TxtOnKey);
            this.Controls.Add(this.BtnStartStop);
            this.Controls.Add(this.BtnAdd);
            this.Controls.Add(this.ListMain);
            this.Controls.Add(this.LabelMoveType);
            this.Controls.Add(this.BtnOnKey);
            this.Controls.Add(this.LabelImageLoc);
            this.Controls.Add(this.ChkClick);
            this.Controls.Add(this.BtnNewKey);
            this.Controls.Add(this.ListMoveType);
            this.Controls.Add(this.BtnImageBrw);
            this.Controls.Add(this.ListImageLoc);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Automation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumericWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericLoopCount)).EndInit();
            this.ContextListMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        internal System.Windows.Forms.Button BtnRecord;
        internal System.Windows.Forms.CheckBox ChkLoopKey;
        internal System.Windows.Forms.NumericUpDown NumericWaitTime;
        internal System.Windows.Forms.NumericUpDown NumericLoopCount;
        internal System.Windows.Forms.Label LabelWaitTime;
        internal System.Windows.Forms.Label LabelMoveSpeed;
        internal System.Windows.Forms.Label LabelLoopCount;
        internal System.Windows.Forms.ComboBox ListMoveSpeed;
        internal System.Windows.Forms.ComboBox ListActionType;
        internal System.Windows.Forms.TextBox TxtNewAction;
        internal System.Windows.Forms.Label LabelOnKey;
        internal System.Windows.Forms.TextBox TxtOnKey;
        internal System.Windows.Forms.ColumnHeader ColumnKey;
        internal System.Windows.Forms.Button BtnStartStop;
        internal System.Windows.Forms.Button BtnAdd;
        internal System.Windows.Forms.ColumnHeader ColumnOrder;
        internal System.Windows.Forms.ColumnHeader ColumnAction;
        internal System.Windows.Forms.ColumnHeader ColumnValue;
        internal System.Windows.Forms.ColumnHeader ColumnLoop;
        internal System.Windows.Forms.ColumnHeader ColumnWait;
        internal System.Windows.Forms.ListView ListMain;
        internal System.Windows.Forms.ColumnHeader ColumnMisc;
        internal System.Windows.Forms.ContextMenuStrip ContextListMain;
        internal System.Windows.Forms.ToolStripMenuItem ContextListMainDelete;
        internal System.Windows.Forms.CheckBox ChkClick;
        internal System.Windows.Forms.Button BtnNewKey;
        internal System.Windows.Forms.Label LabelMoveType;
        internal System.Windows.Forms.Button BtnOnKey;
        internal System.Windows.Forms.Button BtnImageBrw;
        internal System.Windows.Forms.ComboBox ListImageLoc;
        internal System.Windows.Forms.Label LabelImageLoc;
        internal System.Windows.Forms.ComboBox ListMoveType;
    }
}

