namespace Client
{
    partial class ChatForm
    {
        /// <summary>
        /// Обязательный элемент конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освобождает все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">true, если управляемые ресурсы должны быть освобождены; иначе — false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный Windows Form Designer

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxUserName = new TextBox();
            textBoxMessage = new TextBox();
            buttonSend = new Button();
            listBoxMessages = new ListBox();
            buttonAccept = new Button();
            SuspendLayout();
            // 
            // textBoxUserName
            // 
            textBoxUserName.Location = new Point(14, 14);
            textBoxUserName.Margin = new Padding(4, 3, 4, 3);
            textBoxUserName.Name = "textBoxUserName";
            textBoxUserName.Size = new Size(217, 23);
            textBoxUserName.TabIndex = 0;
            textBoxUserName.Text = "Введите ваше имя";
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(14, 266);
            textBoxMessage.Margin = new Padding(4, 3, 4, 3);
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(217, 23);
            textBoxMessage.TabIndex = 1;
            textBoxMessage.Text = "Введите сообщение";
            // 
            // buttonSend
            // 
            buttonSend.Enabled = false;
            buttonSend.Location = new Point(230, 266);
            buttonSend.Margin = new Padding(4, 3, 4, 3);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(88, 27);
            buttonSend.TabIndex = 2;
            buttonSend.Text = "Отправить";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // listBoxMessages
            // 
            listBoxMessages.FormattingEnabled = true;
            listBoxMessages.ItemHeight = 15;
            listBoxMessages.Location = new Point(14, 52);
            listBoxMessages.Margin = new Padding(4, 3, 4, 3);
            listBoxMessages.Name = "listBoxMessages";
            listBoxMessages.Size = new Size(303, 214);
            listBoxMessages.TabIndex = 3;
            // 
            // buttonAccept
            // 
            buttonAccept.Location = new Point(229, 12);
            buttonAccept.Margin = new Padding(4, 3, 4, 3);
            buttonAccept.Name = "buttonAccept";
            buttonAccept.Size = new Size(88, 27);
            buttonAccept.TabIndex = 4;
            buttonAccept.Text = "Подтвердить";
            buttonAccept.UseVisualStyleBackColor = true;
            buttonAccept.Click += buttonAccept_Click;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(331, 301);
            Controls.Add(buttonAccept);
            Controls.Add(listBoxMessages);
            Controls.Add(buttonSend);
            Controls.Add(textBoxMessage);
            Controls.Add(textBoxUserName);
            Margin = new Padding(4, 3, 4, 3);
            Name = "ChatForm";
            Text = "Чат";
            FormClosing += ChatForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.ListBox listBoxMessages;
        private Button buttonAccept;
    }
}