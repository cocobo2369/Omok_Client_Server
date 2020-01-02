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
    public partial class MultiPlayForm : Form
    {

        private const int rectSize = 33;
        private const int edgeCount = 15;

        private bool playing = false;

        private enum Horse { none= 0, BLACK, WHITE };
        private Horse[,] board = new Horse[edgeCount, edgeCount]; //배열 선언을 이렇게 하는 건가봐
        private Horse nowPlayer = Horse.BLACK;
        

        public MultiPlayForm()
        {
            InitializeComponent();
            this.playBtn.Enabled = false; //this.를 붙여주면 playBtn이 이 class의 멤버로 인식하기 좋다.
        }

        private bool judge()
        {
            bool ret = true;
            //가로
            for (int x = 0; x < edgeCount - 4; x++)
            {
                for (int y = 0; y < edgeCount; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x + i, y] != nowPlayer)
                        {
                            ret = false;
                            break;
                        }
                    if (ret == true) return true;
                    else ret = true;
                }
            }

            //세로
            for (int x = 0; x < edgeCount; x++)
            {
                for (int y = 0; y < edgeCount - 4; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x, y + i] != nowPlayer)
                        {
                            ret = false;
                            break;
                        }
                    if (ret == true) return true;
                    else ret = true;
                }
            }

            //대각선 \
            for (int x = 0; x < edgeCount - 4; x++)
            {
                for (int y = 0; y < edgeCount - 4; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x + i, y + i] != nowPlayer)
                        {
                            ret = false;
                            break;
                        }
                    if (ret == true) return true;
                    else ret = true;
                }
            }
            //대각선 /
            for (int x = 0; x < edgeCount - 4; x++)
            {
                for (int y = edgeCount - 1; y > 0; y--)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x + i, y - i] != nowPlayer)
                        {
                            ret = false;
                            break;
                        }
                    if (ret == true) return true;
                    else ret = true;
                }
            }

            return false;
        }


        private void refresh()
        {
            this.boardPicture.Refresh();
            for (int x = 0; x < edgeCount; x++)
                for (int y = 0; y < edgeCount; y++)
                    board[x, y] = Horse.none;
        }


        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (textBox.TextLength == 0)
                status.Text = "들어갈 방 번호를 입력해주세요.";
            else
            {
                status.Text = "[" + textBox.Text + "]" + " 에 접속합니다.";
                connectBtn.Enabled = false;
                playBtn.Enabled = true;
            }
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (playing)
            {
                refresh();
                status.Text = "게임이 재시작 되었습니다.";
            }
            else
            {
                refresh();
                playing = true;
                playBtn.Text = "재시작";
                status.Text = nowPlayer.ToString() + "플레이어의 차례입니다.";
            }
        }

        private void boardPicture_MouseClick(object sender, MouseEventArgs e)
        {
            //1. 플레이 중인가
            if(playing == false)
            {
                MessageBox.Show("게임 시작을 눌러주세요");
                return;
            }

            //2. 가능한 곳을 찍었는가
            Graphics g = this.boardPicture.CreateGraphics();
            int x = e.X / rectSize;
            int y = e.Y / rectSize;
            //e가 MouseClick 이벤트의 argument 니까 Mouseclick의 좌표가 들어올 것임

            if (edgeCount <= x || x < 0 || edgeCount <= y || y < 0)
            {
                MessageBox.Show("테두리를 벗어날 수 없습니다.");
                return;
            }

            if (board[x, y] != Horse.none)
            {
                MessageBox.Show("다른 곳에 놓아주세요");
                return;
            }

            //3. 좌표 저장
            board[x, y] = nowPlayer; //pictureBox의 x,y에 nowPlayer(Black, White) 가 저장될것

            //4. 그리기
            if(nowPlayer == Horse.BLACK)
            {
                SolidBrush brush = new SolidBrush(Color.Black);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.White);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }

            //5. 오목 판단
            if (judge())
            {
                status.Text = nowPlayer.ToString() + "플레이어가 승리했습니다.";
                playing = false;
                playBtn.Text = "게임시작";
            }
            else
            {
                nowPlayer = nowPlayer == Horse.BLACK ? Horse.WHITE : Horse.BLACK;
                status.Text = nowPlayer.ToString() + "플레이어의 차례입니다.";
            }
        }

        private void boardPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            Color lineColor = Color.Black;
           
            //테두리 그리기
            Pen p = new Pen(lineColor, 2);
            //좌측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2);
            //우측
            gp.DrawLine(p, rectSize * edgeCount - rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);
            //상측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize / 2);
            //하측
            gp.DrawLine(p, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);

            //모눈 그리기
            p = new Pen(lineColor, 1);

            for (int i = rectSize + rectSize / 2; i < rectSize * edgeCount - rectSize / 2; i += rectSize)
            {
                gp.DrawLine(p, rectSize / 2, i, rectSize * edgeCount - rectSize / 2, i);
                gp.DrawLine(p, i, rectSize / 2, i, rectSize * edgeCount - rectSize / 2);
            }
        }
    }
}
