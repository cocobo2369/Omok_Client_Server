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
    public partial class SinglePlayForm : Form
    {
        private const int rectSize = 33; //오목판의 셀 크기
        private const int edgeCount = 15; // 오목판의 선 개수

        private bool playing = false;

        private enum Horse {none =0, BLACK, WHITE }; //enum 자체가 자료형이 될 수 있다.
        private Horse[,] board = new Horse[edgeCount, edgeCount];
        private Horse nowPlayer = Horse.BLACK;
        public SinglePlayForm()
        {
            InitializeComponent();
        }

        private bool judge()
        {
            bool ret = true;
            //가로
            for(int x = 0; x < edgeCount - 4; x++)
            {
                for(int y = 0; y < edgeCount; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x+i, y] != nowPlayer)
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
                for (int y = 0; y < edgeCount-4; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x, y+i] != nowPlayer)
                        {
                            ret = false;
                            break;
                        }
                    if (ret == true) return true;
                    else ret = true;
                }
            }

            //대각선 \
            for(int x = 0; x < edgeCount - 4; x++)
            {
                for(int y = 0; y<edgeCount - 4; y++)
                {
                    for(int i = 0;i<5;i++)
                        if(board[x+i,y+i] != nowPlayer)
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

        private void boardPicture_MouseDown(object sender, MouseEventArgs e)
        {
            if(playing == false)
            {
                MessageBox.Show("게임 시작을 눌러주세요");
                return;
            }
            Graphics g = this.boardPicture.CreateGraphics();
            int x = e.X / rectSize;
            int y = e.Y / rectSize;
            if(edgeCount <= x|| x < 0 || edgeCount <= y || y < 0)
            {
                MessageBox.Show("테두리를 벗어날 수 없습니다.");
                return;
            }

            if (board[x, y] != Horse.none)
            {
                MessageBox.Show("다른 곳에 놓아주세요");
                return;
            }
            board[x, y] = nowPlayer;


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
            //오목판 눈금 그리기
            Graphics gp = e.Graphics;
            Color lineColor = Color.Black;
            Pen p = new Pen(lineColor, 2);
            //좌측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2);
            //우측
            gp.DrawLine(p, rectSize * edgeCount - rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);
            //상측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize / 2);
            //하측
            gp.DrawLine(p, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);

            p = new Pen(lineColor, 1);

            for(int i = rectSize + rectSize/2; i <rectSize * edgeCount - rectSize / 2; i += rectSize)
            {
                gp.DrawLine(p, rectSize / 2, i, rectSize * edgeCount - rectSize / 2, i);
                gp.DrawLine(p, i, rectSize / 2, i, rectSize * edgeCount - rectSize / 2);
            }

        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                refresh();
                playing = true;
                playBtn.Text = "재시작";
                status.Text = nowPlayer.ToString() + " 플레이어의 차례입니다.";
            }
            else
            {
                refresh();
                status.Text = "게임이 재시작 되었습니다.";
            }
        }
    }
}
