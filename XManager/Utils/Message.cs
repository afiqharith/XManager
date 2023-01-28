using LogLibrary;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XManager.SystemMessage;

namespace XManager.Utils
{
    internal class Message
    {
        private string _text;
        private string _type;
        private string _button;
        private MessageBoxIcon _mbIcon;
        private MessageBoxButtons _mbButton;
        private DataSet _dataset;
        private Logger _logger = Program.Logger;

        public Message()
        {
            _dataset = GetMessageDatatable();
        }

        private DataSet GetMessageDatatable()
        {
            string xmlString = File.ReadAllText(Convert.ToString(ConfigurationManager.AppSettings["App.MsgXmlRelativePath"]), Encoding.UTF8);
            StringReader strReader = new StringReader(xmlString);
            DataSet dataset = new DataSet();
            dataset.ReadXml(strReader);
            _logger.Trace<Message>(String.Format("Finish {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));
            return dataset;
        }

        private void SetMessageAttr(ID messageId)
        {
            DataRow row = _dataset.Tables["messageBox"].Select(String.Format("id = {0}", (int)messageId))[0];
            int rowIdx = _dataset.Tables["messageBox"].Rows.IndexOf(row);
            _text = Convert.ToString(_dataset.Tables["messageBox"].Rows[rowIdx]["text"]);
            _type = Convert.ToString(_dataset.Tables["messageBox"].Rows[rowIdx]["type"]);
            _button = Convert.ToString(_dataset.Tables["messageBox"].Rows[rowIdx]["button"]);

            switch (_type)
            {
                case "Information":
                    _mbIcon = MessageBoxIcon.Information;
                    break;

                case "Question":
                    _mbIcon = MessageBoxIcon.Question;
                    break;

                case "Warning":
                    _mbIcon = MessageBoxIcon.Warning;
                    break;

                case "Error":
                    _mbIcon = MessageBoxIcon.Error;
                    break;

                default:
                    break;
            }

            switch (_button)
            {
                case "OK":
                    _mbButton = MessageBoxButtons.OK;
                    break;

                case "YesNo":
                    _mbButton = MessageBoxButtons.YesNo;
                    break;

                default:
                    break;
            }
        }

        public DialogResult GetMessageBox(ID messageId)
        {
            SetMessageAttr(messageId);
            DialogResult dialogResult = MessageBox.Show(_text, _type, _mbButton, _mbIcon);
            string buttonTrace = Enum.GetName(typeof(DialogResult), dialogResult);
            string message = String.Format("MBShow - {0}(ID{1}) {2} {3}#{4}", _type, (int)messageId, messageId, buttonTrace, (int)dialogResult);
            _logger.KeyTrace<Message>(buttonTrace);
            _logger.Trace<Message>(message);

            switch (_type)
            {
                case "Information":
                    _logger.Information<Message>(message);
                    break;

                case "Warning":
                    _logger.Warning<Message>(message);
                    break;

                case "Error":
                    _logger.Error<Message>(message);
                    break;

                default:
                    break;
            }

            return dialogResult;
        }

        public DialogResult GetMessageBox(ID messageId, string[] additionalMessage)
        {
            SetMessageAttr(messageId);
            if (additionalMessage.Length > 0)
            {
                for (int i = 0; i < additionalMessage.Length; i++)
                {
                    _text += String.Format("\n{0}", additionalMessage[i]);
                }
            }
            DialogResult dialogResult = MessageBox.Show(_text, _type, _mbButton, _mbIcon);
            string buttonTrace = Enum.GetName(typeof(DialogResult), dialogResult);
            string message = String.Format("MBShow - {0}(ID{1}) {2} {3}#{4}", _type, (int)messageId, _text, buttonTrace, (int)dialogResult);
            _logger.Trace<Message>(message);
            _logger.KeyTrace<Message>(buttonTrace);

            switch (_type)
            {
                case "Information":
                    _logger.Information<Message>(message);
                    break;

                case "Warning":
                    _logger.Warning<Message>(message);
                    break;

                case "Error":
                    _logger.Error<Message>(message);
                    break;

                default:
                    break;
            }

            return dialogResult;
        }

        private void SetMessageString(ID messageId)
        {
            DataRow dataRow = _dataset.Tables["messageStr"].Select(String.Format("id = {0}", (int)messageId))[0];
            int iIndex = _dataset.Tables["messageStr"].Rows.IndexOf(dataRow);
            _text = Convert.ToString(_dataset.Tables["messageStr"].Rows[iIndex]["text"]);
        }

        public string GetMessageString(ID id)
        {
            SetMessageString(id);
            return _text;
        }

        public string GetCurrentMessageString()
        {
            return _text;
        }
    }
}
