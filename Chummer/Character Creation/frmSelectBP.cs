﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace Chummer
{
    public partial class frmSelectBP : Form
    {
        private readonly Character _objCharacter;
        private readonly CharacterOptions _objOptions;
        private bool _blnUseCurrentValues = false;

        #region Control Events
        public frmSelectBP(Character objCharacter, bool blnUseCurrentValues = false)
        {
            _objCharacter = objCharacter;
            _objOptions = _objCharacter.Options;
            _blnUseCurrentValues = blnUseCurrentValues;
            InitializeComponent();
            LanguageManager.Instance.Load(GlobalOptions.Instance.Language, this);

            // Populate the Build Method list.
            List<ListItem> lstBuildMethod = new List<ListItem>();
            ListItem objKarma = new ListItem();
            objKarma.Value = "Karma";
            objKarma.Name = LanguageManager.Instance.GetString("String_Karma");

            ListItem objPriority = new ListItem();
            objPriority.Value = "Priority";
            objPriority.Name = LanguageManager.Instance.GetString("String_Priority");

            ListItem objSumtoTen = new ListItem();
            objSumtoTen.Value = "SumtoTen";
            objSumtoTen.Name = LanguageManager.Instance.GetString("String_SumtoTen");

            lstBuildMethod.Add(objKarma);
            lstBuildMethod.Add(objPriority);
            lstBuildMethod.Add(objSumtoTen);
            cboBuildMethod.DataSource = lstBuildMethod;
            cboBuildMethod.ValueMember = "Value";
            cboBuildMethod.DisplayMember = "Name";

            cboBuildMethod.SelectedValue = _objOptions.BuildMethod;
            nudKarma.Value = _objOptions.BuildPoints;
            nudMaxAvail.Value = _objOptions.Availability;

            // Load the Priority information.

            XmlDocument objXmlDocumentPriority = XmlManager.Instance.Load("priorities.xml");

            // Populate the Gameplay Options list.
            string strDefault = "";
            XmlNodeList objXmlGameplayOptionList = objXmlDocumentPriority.SelectNodes("/chummer/gameplayoptions/gameplayoption");
            foreach (XmlNode objXmlGameplayOption in objXmlGameplayOptionList)
            {
                string strName = objXmlGameplayOption["name"].InnerText;
                try
                {
                    if (objXmlGameplayOption["default"].InnerText == "yes")
                        strDefault = strName;
                }
                catch { }
                ListItem lstGameplay = new ListItem();
                cboGamePlay.Items.Add(strName);
            }
            cboGamePlay.Text = strDefault;

            toolTip1.SetToolTip(chkIgnoreRules, LanguageManager.Instance.GetString("Tip_SelectKarma_IgnoreRules"));

            if (blnUseCurrentValues)
            {
                cboBuildMethod.SelectedValue = "Karma";
                nudKarma.Value = objCharacter.BuildKarma;

                nudMaxAvail.Value = objCharacter.MaximumAvailability;

                cboBuildMethod.Enabled = false;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (cboBuildMethod.SelectedValue.ToString() == "Karma")
            {
                _objCharacter.BuildPoints = 0;
                _objCharacter.BuildKarma = Convert.ToInt32(nudKarma.Value);
                _objCharacter.NuyenMaximumBP = 200;
                _objCharacter.BuildMethod = CharacterBuildMethod.Karma;
                _objCharacter.GameplayOption = cboGamePlay.Text;

            }
            else if (cboBuildMethod.SelectedValue.ToString() == "Priority")
            {
                _objCharacter.BuildPoints = 0;
                _objCharacter.BuildKarma = Convert.ToInt32(nudKarma.Value);
                _objCharacter.NuyenMaximumBP = 10;
                _objCharacter.BuildMethod = CharacterBuildMethod.Priority;
                _objCharacter.GameplayOption = cboGamePlay.Text;
            }
            else if (cboBuildMethod.SelectedValue.ToString() == "SumtoTen")
            {
                _objCharacter.BuildPoints = 0;
                _objCharacter.BuildKarma = Convert.ToInt32(nudKarma.Value);
                _objCharacter.NuyenMaximumBP = 10;
                _objCharacter.BuildMethod = CharacterBuildMethod.SumtoTen;
                _objCharacter.GameplayOption = cboGamePlay.Text;
            }
            else
            {
                MessageBox.Show("All is not well in the state of Denmark.");
            }
            _objCharacter.IgnoreRules = chkIgnoreRules.Checked;
            _objCharacter.MaximumAvailability = Convert.ToInt32(nudMaxAvail.Value);
            this.DialogResult = DialogResult.OK;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cboBuildMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuildMethod.SelectedValue == null)
            {
                lblDescription.Text = LanguageManager.Instance.GetString("String_SelectBP_PrioritySummary");
                nudKarma.Visible = false;
            }
            else
            {
                if (cboBuildMethod.SelectedValue.ToString() == "Karma")
                {
                    if (_objOptions.BuildMethod == "Karma")
                    {
                        lblDescription.Text = LanguageManager.Instance.GetString("String_SelectBP_KarmaSummary").Replace("{0}", _objOptions.BuildPoints.ToString());
                        if (!_blnUseCurrentValues)
                            nudKarma.Value = _objOptions.BuildPoints;
                    }
                    else
                    {
                        lblDescription.Text = LanguageManager.Instance.GetString("String_SelectBP_KarmaSummary").Replace("{0}", "800");
                        if (!_blnUseCurrentValues)
                            nudKarma.Value = 800;
                    }
                    nudKarma.Visible = true;
                    cboGamePlay.Visible = false;
                }
                else if (cboBuildMethod.SelectedValue.ToString() == "Priority")
                {
                    lblDescription.Text = LanguageManager.Instance.GetString("String_SelectBP_PrioritySummary");
                    nudKarma.Visible = false;
                    cboGamePlay.Visible = true;
                }
                else if (cboBuildMethod.SelectedValue.ToString() == "SumtoTen")
                {
                    lblDescription.Text = LanguageManager.Instance.GetString("String_SelectBP_PrioritySummary");
                    nudKarma.Visible = false;
                    cboGamePlay.Visible = true;
                }
            }
        }

        private void frmSelectBP_Load(object sender, EventArgs e)
        {
            this.Height = cmdOK.Bottom + 40;
            cboBuildMethod_SelectedIndexChanged(this, e);
        }
        #endregion

        private void cboGamePlay_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load the Priority information.
            XmlDocument objXmlDocumentPriority = XmlManager.Instance.Load("priorities.xml");

            XmlNode objXmlGameplayOption = objXmlDocumentPriority.SelectSingleNode("/chummer/gameplayoptions/gameplayoption[name = \"" + cboGamePlay.Text + "\"]");
            string strAvail = objXmlGameplayOption["maxavailability"].InnerText;
            nudMaxAvail.Value = Convert.ToInt32(strAvail);
        }
    }
}