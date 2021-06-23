using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FileIOAssignment3
{
    public partial class Form1 : Form
    {

        private string globalFilePath;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridViewXX.ColumnCount = 9;
            this.dataGridViewXX.Columns[0].HeaderText = "MemberID";
            this.dataGridViewXX.Columns[1].HeaderText = "First Name";
            this.dataGridViewXX.Columns[2].HeaderText = "Last Name";
            this.dataGridViewXX.Columns[3].HeaderText = "Date Registered";
            this.dataGridViewXX.Columns[4].HeaderText = "Number of Classes";
            this.dataGridViewXX.Columns[5].HeaderText = "Total Cost Per Class";
            this.dataGridViewXX.Columns[6].HeaderText = "Total cost of all classes";
            this.dataGridViewXX.Columns[7].HeaderText = "Total Paid";
            this.dataGridViewXX.Columns[8].HeaderText = "Amount Outstanding";

            lblError.Text = "";
        }

        private void btnCFExistsXX_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            string filePath = txtFilePathXX.Text;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                lblError.Text = "file path and name field is required!\n";
                changeAbleButton(false);
            }
            else if (File.Exists(filePath))
            {
                globalFilePath = filePath;
                showData();
                changeAbleButton(true);
            } else
            {
                try
                {
                    Regex txtFilePattern = new Regex(@".*(\.txt)");
                    if (!txtFilePattern.IsMatch(filePath))
                    {
                        lblError.Text = "plase enter a txt file";
                    } else
                    {
                        string directoryPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directoryPath) && directoryPath != "")
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        File.Create(filePath).Dispose();
                        lblError.Text = "new file is created";
                        globalFilePath = filePath;
                        changeAbleButton(true);
                        
                    }
                    
                } catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    changeAbleButton(false);
                }
                

            }
        }

        private void btnExitXX_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSaveXX_Click(object sender, EventArgs e)
        {
            string merberId = txtMIDXX.Text;
            string firstName = txtFNameXX.Text;
            string lastName = txtLNameXX.Text;
            string dateRegistered = dPickerXX.Text;
            string numberOfClasses = txtNOClassesXX.Text;
            string tCPClass = txtTCPClassXX.Text;
            string tPaid = txtTPaidXX.Text;
            string tCOAClassses = txtTCOAClassesXX.Text;
            string aOutstanding = txtAOustandingXX.Text;

            if (validationData())
            {
                bool hasId = checkMemberId(merberId);
                try
                {

                    string record = createRecord(merberId, firstName, lastName, dateRegistered, numberOfClasses, tCPClass, tPaid, tCOAClassses, aOutstanding);
                    if (hasId)
                    {
                        int index = returnIndexOfMemberId(merberId);
                        List<string> records = new List<string> { };
                        if (index != -1)
                        {
                            using (StreamReader reader = new StreamReader(globalFilePath))
                            {
                                while (!reader.EndOfStream)
                                {
                                    records.Add(reader.ReadLine());
                                }
                            }

                            using (StreamWriter write = new StreamWriter(globalFilePath))
                            {
                                for (int i = 0; i < records.Count(); i++)
                                {
                                    if (i != index)
                                    {
                                        write.WriteLine(records[i]);
                                    }
                                    else if (i == index)
                                    {
                                        write.WriteLine(record);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        using (StreamWriter write = new StreamWriter(globalFilePath, append: true))
                        {
                            write.WriteLine(record);
                        }
                    }

                    showData();
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                }
            }
            //save or update
            

        }

        private void btnDeleteXX_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            string merberId = txtMIDXX.Text;
            int index = returnIndexOfMemberId(merberId);
            List<string> records = new List<string> { };
            if (index != -1)
            {
                using (StreamReader reader = new StreamReader(globalFilePath))
                {
                    while(!reader.EndOfStream)
                    {
                        records.Add(reader.ReadLine());
                    }
                }

                using (StreamWriter writer = new StreamWriter(globalFilePath))
                {
                    for(int i = 0; i <　records.Count(); i++)
                    {
                        if (i != index)
                        {
                            writer.WriteLine(records[i]);
                        }
                    }
                }
                showData();
            }
        }

        private void btnEmptyFileXX_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            try
            {
                using (FileStream fileOpen = new FileStream(globalFilePath, FileMode.Truncate)) { }
                showData();
            } catch(Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void btnReloadXX_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            showData();
        }

        private void changeAbleButton(bool input)
        {
            if (input)
            {
                btnDeleteXX.Enabled = true;
                btnEmptyFileXX.Enabled = true;
                btnReloadXX.Enabled = true;
                btnSaveXX.Enabled = true;
            }
            else
            {
                btnDeleteXX.Enabled = false;
                btnEmptyFileXX.Enabled = false;
                btnReloadXX.Enabled = false;
                btnSaveXX.Enabled = false;
            }
        }

        private void showData()
        {
            try
            {
                dataGridViewXX.Rows.Clear();
                TextInfo firstUpper = new CultureInfo("en-US", false).TextInfo;
                using (StreamReader reader = new StreamReader(globalFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string record = reader.ReadLine();
                        if (record != "")
                        {
                            string[] fields = record.Split('\t');
                            int index = dataGridViewXX.Rows.Add();
                            dataGridViewXX.Rows[index].Cells[0].Value = fields[0].ToUpper();
                            dataGridViewXX.Rows[index].Cells[1].Value = firstUpper.ToTitleCase(fields[1]);
                            dataGridViewXX.Rows[index].Cells[2].Value = firstUpper.ToTitleCase(fields[2]);
                            dataGridViewXX.Rows[index].Cells[3].Value = fields[3];
                            dataGridViewXX.Rows[index].Cells[4].Value = fields[4];
                            dataGridViewXX.Rows[index].Cells[5].Value = "$" + fields[5];
                            dataGridViewXX.Rows[index].Cells[6].Value = "$" + fields[6];
                            dataGridViewXX.Rows[index].Cells[7].Value = "$" + fields[7];
                            dataGridViewXX.Rows[index].Cells[8].Value = "$" + fields[8];
                        }
                        
                    }
                }
            } catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
            
        }

        private bool validationData()
        {
            string merberId = txtMIDXX.Text;
            string firstName = txtFNameXX.Text;
            string lastName = txtLNameXX.Text;
            DateTime dateRegistered = dPickerXX.Value;
            string numberOfClasses = txtNOClassesXX.Text;
            string tCPClass = txtTCPClassXX.Text;
            string tPaid = txtTPaidXX.Text;
            string tcoaClasses = txtTCOAClassesXX.Text;
            string aOutstanding = txtAOustandingXX.Text;
            string error = "";
            bool isFocus = true;

            Regex memberIdPattern = new Regex(@"^[\da-zA-Z]+$");
            if (string.IsNullOrWhiteSpace(merberId))
            {
                error += "member Id is required!\n";
                if (isFocus)
                {
                    txtMIDXX.Focus();
                    isFocus = false;
                }
            } else if (!memberIdPattern.IsMatch(merberId))
            {
                error += "member Id must contain only letters and numbers\n";
                if (isFocus)
                {
                    txtMIDXX.Focus();
                    isFocus = false;
                }
            }

            Regex namePattern = new Regex(@"^(?!.*\d)[a-zA-Z]{2,}$");
            if (string.IsNullOrWhiteSpace(firstName))
            {
                error += "first name is required!\n";
                if (isFocus)
                {
                    txtFNameXX.Focus();
                    isFocus = false;
                }
            } else if (!namePattern.IsMatch(firstName))
            {
                error += "first name must be more than 2 characters and must not contain numbers\n";
                if (isFocus)
                {
                    txtFNameXX.Focus();
                    isFocus = false;
                }
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                error += "last name is required!\n";
                if (isFocus)
                {
                    txtLNameXX.Focus();
                    isFocus = false;
                }
            }
            else if (!namePattern.IsMatch(lastName))
            {
                error += "last name must be more than 2 characters and must not contain numbers\n";
                if (isFocus)
                {
                    txtLNameXX.Focus();
                    isFocus = false;
                }
            }

            DateTime current = DateTime.Now;
            if (current < dateRegistered)
            {
                error += "date registered must not be in the future\n";
                if (isFocus)
                {
                    dPickerXX.Focus();
                    isFocus = false;
                }
            }

            try
            {
                int numberOfClassesInt = Convert.ToInt32(numberOfClasses);
                if (numberOfClassesInt < 1)
                {
                    error += "number of classes must be greater than or equal to 1";
                    if (isFocus)
                    {
                        txtNOClassesXX.Focus();
                        isFocus = false;
                    }
                }
            } catch
            {
                error += "number of classes is invalid\n";
                if (isFocus)
                {
                    txtNOClassesXX.Focus();
                    isFocus = false;
                }
            }

            try
            {
                decimal totalCPCDecimal = Convert.ToDecimal(tCPClass);
                if (totalCPCDecimal < 1)
                {
                    error += "total cost per class must be greater than or equal to 1";
                    if (isFocus)
                    {
                        txtTCPClassXX.Focus();
                        isFocus = false;
                    }
                }
            }
            catch
            {
                error += "total cost per class is invalid\n";
                if (isFocus)
                {
                    txtTCPClassXX.Focus();
                    isFocus = false;
                }
            }

            Regex checkIsNumber = new Regex(@"^\d+$");
            if (!checkIsNumber.IsMatch(tPaid))
            {
                error += "total paid must be a number\n";
                if (isFocus)
                {
                    txtTPaidXX.Focus();
                    isFocus = false;
                }
            }

            if (!string.IsNullOrEmpty(tcoaClasses))
            {
                if (!checkIsNumber.IsMatch(tcoaClasses))
                {
                    error += "total cost of all classes must be a number\n";
                    if (isFocus)
                    {
                        txtTCOAClassesXX.Focus();
                        isFocus = false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(aOutstanding))
            {
                if (!checkIsNumber.IsMatch(aOutstanding))
                {
                    error += "amount outstanding must be a number\n";
                    if (isFocus)
                    {
                        txtAOustandingXX.Focus();
                        isFocus = false;
                    }
                }
            }

            if (error != "")
            {
                lblError.Text = error;
                return false;
            } else
            {
                return true;
            }








        }

        private string createRecord(string mid, string fname, string lname, string dr, string noc, string tcpc, string tp, string tcoac, string ao) 
        {
            if (string.IsNullOrEmpty(tcoac))
            {
                decimal tcoacDecimal = Convert.ToDecimal(noc) * Convert.ToDecimal(tcpc);
                tcoac = Math.Round(tcoacDecimal, 2).ToString();
            }
            if (string.IsNullOrEmpty(ao))
            {
                decimal aoDecimal = Convert.ToDecimal(tp) - Convert.ToDecimal(tcoac);
                ao = Math.Round(aoDecimal, 2).ToString();
            }
            return $"{mid}\t{fname}\t{lname}\t{dr}\t{noc}\t{tcpc}\t{tcoac}\t{tp}\t{ao}";
        }

        private int returnIndexOfMemberId(string enterMid)
        {
            int index = -1;
            try {
                using (StreamReader reader = new StreamReader(globalFilePath))
                {
                    for (int i = 0; !reader.EndOfStream; i++)
                    {
                        string record = reader.ReadLine();
                        if (record.StartsWith(enterMid))
                        {
                            index = i;
                            break;
                        }
                    }
                }
            } catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
            return index;
            
        }

        private bool checkMemberId(string enterMId)
        {
            bool hasId = false;
            using (StreamReader reader = new StreamReader(globalFilePath))
            {
                while(!reader.EndOfStream)
                {
                    string record = reader.ReadLine();
                    string[] records = record.Split('\t');
                    if (enterMId == records[0])
                    {
                        hasId = true;
                        break;
                    }
                }
            }
            return hasId;
        }
    }
}
