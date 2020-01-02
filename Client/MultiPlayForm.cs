using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public partial class MultiPlayForm : Form
    {
        private Thread thread; //통신쓰레드
        private TcpClient tcpClient;
        private NetworkStream stream;

        private const int rectSize = 33;
        private const int edgeCount = 15;

        private bool playing;
        private bool entered;
        private bool threading;
        private bool nowTurn;

        private enum Horse { none = 0, BLACK, WHITE };
        private Horse[,] board;
        private Horse nowPlayer;


        public MultiPlayForm()
        {
            InitializeComponent();
            this.playBtn.Enabled = false; //this.를 붙여주면 playBtn이 이 class의 멤버로 인식하기 좋다.
            playing = false;
            entered = false;
            threading = false;
            nowTurn = false;
            board = new Horse[edgeCount, edgeCount]; //배열 선언을 이렇게 하는 건가봐
        }

        private bool judge(Horse player)
        {
            bool ret = true;
            //가로
            for (int x = 0; x < edgeCount - 4; x++)
            {
                for (int y = 0; y < edgeCount; y++)
                {
                    for (int i = 0; i < 5; i++)
                        if (board[x + i, y] != player)
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
                        if (board[x, y + i] != player)
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
                        if (board[x + i, y + i] != player)
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
                        if (board[x + i, y - i] != player)
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
            playBtn.Enabled = false;
        }


        private void connectBtn_Click(object sender, EventArgs e)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 9876);
            stream = tcpClient.GetStream();

            thread = new Thread(new ThreadStart(read));
            thread.Start();
            threading = true;

            //방접속
            string message = "[Enter]";
            byte[] buf = Encoding.ASCII.GetBytes(message + this.textBox.Text);
            stream.Write(buf, 0, buf.Length);
        }

        //서버로부터 메세지 받음
        private void read()
        {
            while (true)
            {
                byte[] buf = new byte[1024];
                int bufBytes = stream.Read(buf, 0, buf.Length);
                string message = Encoding.ASCII.GetString(buf, 0, bufBytes);
                /* 접속 성공 (메시지: [Enter]) */
                if (message.Contains("[Enter]"))
                {
                    this.status.Text = "[" + this.textBox.Text + "]번 방에 접속했습니다.";
                    /* 게임 시작 처리 */
                    this.textBox.Enabled = false;
                    this.connectBtn.Enabled = false;
                    entered = true;
                }
                /* 방이 가득 찬 경우 (메시지: [Full]) */
                if (message.Contains("[Full]"))
                {
                    this.status.Text = "이미 가득 찬 방입니다.";
                    closeNetwork();
                }
                /* 게임 시작 (메시지: [Play]{Horse}) */
                if (message.Contains("[Play]"))
                {
                    refresh();
                    string horse = message.Split(']')[1];
                    if (horse.Contains("Black"))
                    {
                        this.status.Text = "당신의 차례입니다.";
                        nowTurn = true;
                        nowPlayer = Horse.BLACK;
                    }
                    else
                    {
                        this.status.Text = "상대방의 차례입니다.";
                        nowTurn = false;
                        nowPlayer = Horse.WHITE;
                    }
                    playing = true;
                }
                /* 상대방이 나간 경우 (메시지: [Exit]) */
                if (message.Contains("[Exit]"))
                {
                    this.status.Text = "상대방이 나갔습니다.";
                    refresh();
                }
                /* 상대방이 돌을 둔 경우 (메시지: [Axis]{X,Y}) */
                if (message.Contains("[Axis]"))
                {
                    string position = message.Split(']')[1];
                    int x = Convert.ToInt32(position.Split(',')[0]);
                    int y = Convert.ToInt32(position.Split(',')[1]);
                    Horse enemyPlayer = Horse.none;
                    if (nowPlayer == Horse.BLACK)
                    {
                        enemyPlayer = Horse.WHITE;
                    }
                    else
                    {
                        enemyPlayer = Horse.BLACK;
                    }
                    if (board[x, y] != Horse.none) continue;
                    board[x, y] = enemyPlayer;
                    Graphics g = this.boardPicture.CreateGraphics();
                    if (enemyPlayer == Horse.BLACK)
                    {
                        SolidBrush brush = new SolidBrush(Color.Black);
                        g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
                    }
                    else
                    {
                        SolidBrush brush = new SolidBrush(Color.White);
                        g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
                    }
                    if (judge(enemyPlayer))
                    {
                        status.Text = "패배했습니다.";
                        playing = false;
                        playBtn.Text = "재시작";
                        playBtn.Enabled = true;
                    }
                    else
                    {
                        status.Text = "당신이 둘 차례입니다.";
                    }
                    nowTurn = true;
                }
            }
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                refresh();
                playing = true;
                string message = "[Play]";
                byte[] buf = Encoding.ASCII.GetBytes(message + this.textBox.Text);
                stream.Write(buf, 0, buf.Length);
                this.status.Text = "상대 플레이어의 준비를 기다립니다.";
                this.playBtn.Enabled = false;
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


        void closeNetwork()
        {
            if (threading && thread.IsAlive) thread.Abort();
            if (entered)
                tcpClient.Close();
        }

        private void MultiPlayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeNetwork();
        }

        private void boardPicture_MouseDown(object sender, MouseEventArgs e)
        {
            //1. 플레이 중인가
            if (playing == false)
            {
                MessageBox.Show("게임 시작을 눌러주세요");
                return;
            }
            if (!nowTurn)
                return;

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
            if (nowPlayer == Horse.BLACK)
            {
                SolidBrush brush = new SolidBrush(Color.Black);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.White);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }

            /* 놓은 바둑돌의 위치 보내기 */
            string message = "[Put]" + textBox.Text + "," + x + "," + y;
            byte[] buf = Encoding.ASCII.GetBytes(message);
            stream.Write(buf, 0, buf.Length);
            //5. 오목 판단
            if (judge(nowPlayer))
            {
                status.Text = "승리했습니다.";
                playing = false;
                playBtn.Text = "재시작";
                playBtn.Enabled = true;
                return;
            }
            else
            {
                status.Text = "상대방이 둘 차례입니다.";
            }
            /* 상대방의 차레로 설정하기 */
            nowTurn = false;
        }


    }

}
