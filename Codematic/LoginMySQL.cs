using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Maticsoft.CmConfig;
namespace Codematic
{
    public partial class LoginMySQL : Form
    {
        Maticsoft.CmConfig.DbSettings dbobj = new Maticsoft.CmConfig.DbSettings();
        public string constr;
        public string dbname = "mysql";

        public LoginMySQL()
        {
            InitializeComponent();
        }

        private void btn_ConTest_Click(object sender, EventArgs e)
        {
            try
            {
                string server = this.comboBoxServer.Text.Trim();
                string user = this.txtUser.Text.Trim();
                string pass = this.txtPass.Text.Trim();
                string port = this.textBox1.Text.Trim();
                if ((user == "") || (server == ""))
                {
                    MessageBox.Show(this, "���������û�������Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    constr = String.Format("server={0};user id={1}; Port={2};password={3}; pooling=false", server, user, port, pass);
                    try
                    {
                        this.Text = "�������ӷ����������Ժ�...";
                        Maticsoft.IDBO.IDbObject dbobj = Maticsoft.DBFactory.DBOMaker.CreateDbObj("MySQL");
                        dbobj.DbConnectStr = constr;
                        List<string> dblist = dbobj.GetDBList();
                        this.cmbDBlist.Enabled = true;
                        this.cmbDBlist.Items.Clear();
                        this.cmbDBlist.Items.Add("ȫ����");//5_1_a_s_p_x
                        if (dblist != null && dblist.Count > 0)
                        {
                            foreach (string dbname in dblist)
                            {
                                this.cmbDBlist.Items.Add(dbname);
                            }
                        }
                        this.cmbDBlist.SelectedIndex = 0;
                        this.Text = "���ӷ������ɹ���";

                    }
                    catch (System.Exception ex)
                    {
                        LogInfo.WriteLog(ex);
                        this.Text = "���ӷ��������ȡ������Ϣʧ�ܣ�";
                        DialogResult drs = MessageBox.Show(this, "���ӷ��������ȡ������Ϣʧ�ܣ�\r\n�����������ַ���û��������Ƿ���ȷ���鿴�����ļ��԰�����������⣿", "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
                        //if (drs == DialogResult.OK)
                        //{
                        //    try
                        //    {
                        //        //Process proc = new Process();
                        //        //Process.Start("IExplore.exe", "http://help.maticsoft.com");
                        //    }
                        //    catch
                        //    {
                        //        MessageBox.Show("����ʣ�http://www.maticsoft.com", "���", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        //    }
                        //}
                        //return;
                    }

                }
            }
            catch (Exception ex2)
            {
                LogInfo.WriteLog(ex2);
                MessageBox.Show(this, ex2.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string server = this.comboBoxServer.Text.Trim();
                string user = this.txtUser.Text.Trim();
                string pass = this.txtPass.Text.Trim();
                string port = this.textBox1.Text.Trim();
                if (user == "" || server == "")
                {
                    MessageBox.Show(this, "���������û�������Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    if (this.cmbDBlist.SelectedIndex > 0)
                    {
                        dbname = cmbDBlist.Text;
                    }
                    else
                    {
                        dbname = "mysql";
                    }
                    constr = String.Format("server={0};user id={1}; Port={2};password={3}; database={4}; pooling=false", server, user, port, pass, dbname);
                    //��������
                    MySqlConnection myCn = new MySqlConnection(constr);
                    try
                    {
                        this.Text = "�������ӷ����������Ժ�...";
                        myCn.Open();
                    }
                    catch (System.Exception ex)
                    {
                        LogInfo.WriteLog(ex);
                        this.Text = "���ӷ�����ʧ�ܣ�";
                        MessageBox.Show(this, "���ӷ�����ʧ�ܣ������������ַ���û��������Ƿ���ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }
                    finally
                    {
                        myCn.Close();
                    }
                    this.Text = "���ӷ������ɹ���";
                    if (dbobj == null)
                    {
                        dbobj = new Maticsoft.CmConfig.DbSettings();
                    }
                    string strtype = "MySQL";
                    //����ǰ����д�������ļ�
                    dbobj.DbType = strtype;
                    dbobj.Server = server;
                    dbobj.ConnectStr = constr;
                    dbobj.DbName = dbname;
                    this.dbobj.DbHelperName = "DbHelperMySQL";
                    dbobj.ConnectSimple = chk_Simple.Checked;
                    switch (DbConfig.AddSettings(this.dbobj))
                    {
                        case 0:
                            MessageBox.Show(this, "��ӷ���������ʧ�ܣ����鰲װĿ¼�Ƿ���д��Ȩ�޻��ļ��Ƿ���ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            return;
                        case 2:
                            {
                                DialogResult dialogResult = MessageBox.Show(this, "�÷�������Ϣ�Ѿ����ڣ���ȷ���Ƿ񸲸ǵ�ǰ���ݿ����ã�", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                                if (dialogResult != DialogResult.Yes)
                                {
                                    return;
                                }
                                DbConfig.DelSetting(this.dbobj.DbType, this.dbobj.Server, this.dbobj.DbName);
                                int num = DbConfig.AddSettings(this.dbobj);
                                if (num != 1)
                                {
                                    MessageBox.Show(this, "����ж�ص�ǰ�汾����ɾ����װĿ¼�����°�װ���°汾��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    return;
                                }
                                break;
                            }
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LogInfo.WriteLog(ex);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoginMySQL_Load(object sender, EventArgs e)
        {
            //comboBoxServerVer.SelectedIndex = 0;
            //comboBox_Verified.SelectedIndex = 0;
        }
    }
}