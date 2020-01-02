using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void SinglePlayBtn_Click(object sender, EventArgs e)
        {
            Hide(); //현재창을 숨김
            SinglePlayForm singlePlayForm = new SinglePlayForm(); //singleplayform의 instance 생성
            singlePlayForm.FormClosed += new FormClosedEventHandler(childForm_Closed); //이벤트 핸들러로 singlePlayForm이 닫히면 --> 이 이벤트가 childForm_Closed 라는 함수를 호출해서 MenuForm이 호출됨
            singlePlayForm.Show();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        void childForm_Closed(object sender, FormClosedEventArgs e) //얘는 함수
        {
            Show();
        }

        private void MultiPlayBtn_Click(object sender, EventArgs e)
        {
            Hide();
            MultiPlayForm multiPlayForm = new MultiPlayForm();
            multiPlayForm.FormClosed += new FormClosedEventHandler(childForm_Closed);
            multiPlayForm.Show();
        }
    }
}
