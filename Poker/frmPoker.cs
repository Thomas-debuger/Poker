using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        #region 欄位
        /// <summary>
        /// 用來存放牌桌上五張牌的 PictureBox 陣列
        /// </summary>
        PictureBox[] pic = new PictureBox[5];

        /// <summary>
        /// 所有的牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] allPoker = new int[52];

        /// <summary>
        /// 記錄玩家手牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] playerPoker = new int[5];

        #endregion

        #region 資金變數
        /// <summary>
        /// 玩家的總資金
        /// </summary>
        int totalMoney = 1000000;

        /// <summary>
        /// 單次押注金額
        /// </summary>
        int betAmount = 500;

        // --- 新增：用來記錄每一局遊戲結果的清單 ---
        List<string> gameHistory = new List<string>();

        #endregion


        public frmPoker()
        {

            InitializeComponent();
            InitializePoker();
        }


        #region 自定義方法
        private void InitializePoker()
        {
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                // 預設牌桌上的牌不可點擊
                pic[i].Enabled = false;
                // 預設牌桌上的牌的 Tag 為 "back"，表示牌面朝下
                pic[i].Tag = "back";
                pic[i].Visible = true;

                // 將 pic 丟至到 grpPorker 內
                this.grpPoker.Controls.Add(pic[i]);

                pic[i].Click += Pic_Click;
            }
        }

        /// <summary>
        /// 顯示五張撲克牌到桌面上
        /// </summary>
        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
        }


        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="name">string 的牌名 </param>
        /// <returns></returns>
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        /// <param name="num">撲克牌編號</param>
        /// <returns></returns>
        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }


        /// <summary>
        /// 將 allPoker 陣列中的牌隨機打亂，模擬洗牌的過程
        /// </summary>
        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[0];
                allPoker[0] = temp;
            }
        }

        #endregion


        #region 事件處理程序

        /// <summary>
        /// 牌桌上的牌被按下時，顯示訊息框告訴使用者按下了哪一張牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;


            int index = int.Parse(pic.Name.Replace("pic", ""));

            int cardNum = playerPoker[index] + 1;

            // 如果牌面朝下，則翻開牌面；如果牌面朝上，則翻回背面
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage(cardNum);
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }

        /// <summary>
        /// 當按下發牌按鈕時，隨機產生五個1~52的數字，並將對應的圖片顯示在牌桌上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            // 將上一把玩的結果清除
            this.lblResult.Text = "";


            // 將牌桌上的牌重置為背面圖
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
            }

            // 將所有牌的編號從 0 到 51 填入 allPoker 陣列
            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            // 洗牌
            this.Shuffle();

            // 暫停500ms
            await Task.Delay(500);

            // 發前五張牌給玩家，並將對應的牌面圖顯示在牌桌上
            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 取前52張牌的前五張牌
                playerPoker[i] = allPoker[i];
            }


            //// 測試用
            //playerPoker[0] = 51;
            //playerPoker[1] = 47;
            //playerPoker[2] = 43;
            //playerPoker[3] = 39;
            //playerPoker[4] = 3;


            // 將對應的牌面圖顯示在牌桌上
            this.ShowCards();

            // 啟用所有牌的點擊事件
            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成可以點擊
                pic[i].Enabled = true;
                // 將牌桌上的牌的 Tag 設成 "front"，表示牌面朝上
                pic[i].Tag = "front";
            }

            // 啟用換牌按鈕
            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;

        }

        /// <summary>
        /// 當按下換牌按鈕時，將玩家手牌中被選中的牌換成新的牌，並將對應的圖片顯示在牌桌上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5; // 從 allPoker 陣列的第 5 張牌開始換牌，因為前 5 張牌已經發給玩家了

            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 如果牌面朝下，表示玩家選擇換掉這張牌
                if (pic[i].Tag.ToString() == "back")
                {
                    // 將玩家手牌中被選中的牌換成新的牌
                    playerPoker[i] = allPoker[startIndex];
                    // 將對應的牌面圖顯示在牌桌上
                    pic[i].Image = GetImage(playerPoker[i] + 1);
                    pic[i].Tag = "front";

                    startIndex++;
                }
            }

            for (int i = 0; i < pic.Length; i++)
            {
                // 將牌桌上的牌設成不可點擊
                pic[i].Enabled = false;
            }

            // 將換牌按鈕設成不可用，表示玩家已經完成換牌了
            this.btnChangeCard.Enabled = false;

            // 將判斷牌型的按鈕設成可用，表示玩家可以開始判斷牌型了
            this.btnCheck.Enabled = true;
        }

        /// <summary>
        /// 當按下判斷牌型按鈕時，根據玩家手牌的編號，判斷玩家的牌型，並顯示在 lblResult 上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };


            // 計錄目前五張撲克牌的花色的陣列
            int[] pokerColor = new int[5];
            // 計錄目前五張撲克牌的點數的陣列
            int[] pokerPoint = new int[5];


            for (int i = 0; i < playerPoker.Length; i++)
            {
                // 根據玩家手牌的編號，計算出玩家手牌的花色 
                pokerColor[i] = playerPoker[i] % 4;
                // 根據玩家手牌的編號，計算出玩家手牌的點數
                pokerPoint[i] = playerPoker[i] / 4;
            }

            #region 測試計算出來的花色和點數是否正確
            //=================================================
            //string result = "";
            //for (int i = 0; i < playerPoker.Length; i++)
            //{
            //    // 取得花色編號
            //    int iColor = pokerColor[i];
            //    // 取得點數編號
            //    int iPoint = pokerPoint[i];
            //    // 根據花色編號和點數編號，組合成牌的名稱
            //    result += $"{colorList[iColor]}{pointList[iPoint]} ";
            //}
            //// 顯示玩家撲克牌的花色和點數
            //this.lblResult.Text = result;
            //=================================================
            #endregion

            // 記錄花色和點數出現次數的陣列
            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            // 統計 color 和 point 出現次數
            for (int i = 0; i < pokerColor.Length; i++)
            {
                int color = pokerColor[i];
                int point = pokerPoint[i];

                colorCount[color]++;
                pointCount[point]++;
            }

            // 將 colorCount 和 colorList 兩個陣列一起排序，根據 colorCount 的值從小到大排序，並且保持 colorList 的對應關係
            Array.Sort(colorCount, colorList);
            Array.Reverse(colorCount);
            Array.Reverse(colorList);

            Array.Sort(pointCount, pointList);
            Array.Reverse(pointCount);
            Array.Reverse(pointList);


            // 判斷是否為同花
            bool isFlush = (colorCount[0] == 5);
            // 判斷是否為五張單張
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 && pointCount[3] == 1 && pointCount[4] == 1);
            // 判斷是否為差四
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            // 判斷是否為大順
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) && pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            // 判斷是否為同花大順
            bool isRoyalisFlush = isFlush && isRoyal;
            // 判斷是否為同花順
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            // 判斷是否為順子
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            // 判斷是否為鐵支
            bool isFourOfAKind = (pointCount[0] == 4);
            // 判斷是否為葫蘆
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            // 判斷是否為三條
            bool isThreeOfAKind = (pointCount[0] == 3 && pointCount[1] == 1);
            // 判斷是否為兩對
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            // 判斷是否為一對
            bool isOnePair = (pointCount[0] == 2 && pointCount[1] == 1);

            string result = "";
            int odds = 0; // 記錄對應的賠率

            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 皇家同花順"; // 依照文件調整名稱 
                odds = 250; // 
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
                odds = 50; // 
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 四條"; // 依照文件調整名稱 
                odds = 25; // 
            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
                odds = 9; // 
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
                odds = 6; // 
            }
            else if (isStraight)
            {
                result = "順子";
                odds = 4; // 
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
                odds = 3; // 
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]},{pointList[1]} 兩對";
                odds = 2; // 
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
                odds = 1; // 
            }
            else
            {
                result = "雜牌";
                odds = 0;
            }

            // 根據牌型賠率計算中獎金額 
            int winAmount = betAmount * odds;
            totalMoney += winAmount;
            lblTotalMoney.Text = totalMoney.ToString(); // 更新 UI 上的總資金

            // 顯示最終結果與獎金
            if (odds > 0)
            {
                result += $"\r\n恭喜中獎！獲得 {winAmount} 元";
            }
            else
            {
                result += "\r\n未中獎，請再接再厲！";
            }

            lblResult.Text = result;

            // ================= 新增這一段：紀錄遊戲歷史 =================
            // 假設 result 變數裡面存的是 "梅花 皇家同花順\r\n恭喜中獎！獲得 125000 元"
            // 我們把它整理成一行文字存起來
            string cleanResult = result.Replace("\r\n", " | ");
            string record = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 押注: {betAmount} | 結果: {cleanResult} | 結餘: {totalMoney}";

            gameHistory.Add(record); // 把這一局的紀錄存入清單
                                     // ==========================================================

            // 回復按鈕狀態，準備進入下一局
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
            btnDealCard.Enabled = false;
            btnBet.Enabled = true;
            txtBetAmount.Enabled = true;
        }

        /// <summary>
        /// 當表單被按下鍵盤時觸發
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.btnDealCard.Enabled == false)
            {
                switch (e.KeyChar)
                {
                    case 'q':
                        // 同花大順
                        playerPoker[0] = 51;
                        playerPoker[1] = 47;
                        playerPoker[2] = 43;
                        playerPoker[3] = 39;
                        playerPoker[4] = 3;

                        break;
                    case 'w':
                        // 同花順
                        playerPoker[0] = 37;
                        playerPoker[1] = 33;
                        playerPoker[2] = 29;
                        playerPoker[3] = 25;
                        playerPoker[4] = 21;
                        break;
                    case 'e':
                        // 同花
                        playerPoker[0] = 50;
                        playerPoker[1] = 38;
                        playerPoker[2] = 34;
                        playerPoker[3] = 22;
                        playerPoker[4] = 18;
                        break;
                    case 'r':
                        // 鐵支
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 38;
                        playerPoker[3] = 37;
                        playerPoker[4] = 36;
                        break;
                    case 't':
                        // 葫蘆
                        playerPoker[0] = 30;
                        playerPoker[1] = 29;
                        playerPoker[2] = 6;
                        playerPoker[3] = 5;
                        playerPoker[4] = 4;
                        break;
                    case 'y':
                        // 三條
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 15;
                        playerPoker[3] = 14;
                        playerPoker[4] = 13;
                        break;
                }

                // 顯示五張撲克牌到桌面上
                this.ShowCards();
            }
        }

        /// <summary>
        /// 當按下押注按鈕時，扣除資金並開放發牌
        /// </summary>
        private void btnBet_Click(object sender, EventArgs e)
        {
            // 假設你的輸入框叫做 txtBetAmount
            if (int.TryParse(txtBetAmount.Text, out betAmount))
            {
                if (betAmount > 0 && betAmount <= totalMoney)
                {
                    // 扣除押注金額並更新介面
                    totalMoney -= betAmount;

                    // 假設你的總資金顯示區叫做 lblTotalMoney (或 txtTotalMoney)
                    lblTotalMoney.Text = totalMoney.ToString();

                    // 鎖定下注介面，準備發牌
                    btnBet.Enabled = false;
                    txtBetAmount.Enabled = false;
                    btnDealCard.Enabled = true; // 開放發牌按鈕

                    lblResult.Text = "已押注，請點擊「發牌」";
                }
                else
                {
                    MessageBox.Show("押注金額錯誤或餘額不足！");
                }
            }
            else
            {
                MessageBox.Show("請輸入正確的數字！");
            }
        }
        #endregion

        private void btnColorMenu_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog(); // 呼叫內建的調色盤

            // 如果使用者選好顏色並按下確定
            if (cd.ShowDialog() == DialogResult.OK)
            {
                // 改變整個視窗的背景顏色
                this.BackColor = cd.Color;

                // 改變所有按鈕的背景顏色 (使用我們自己寫的遞迴方法)
                ChangeButtonColor(this, cd.Color);
            }
        }
        // 搭配調色功能使用：自動尋找視窗內所有按鈕並改色
        // (因為按鈕被放在 GroupBox 裡面，需要用遞迴尋找)
        private void ChangeButtonColor(Control parent, Color newColor)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Button)
                {
                    // 將通用的 Control 強制轉型為 Button
                    Button btn = (Button)c;

                    btn.BackColor = newColor; // 改按鈕顏色
                    btn.UseVisualStyleBackColor = false; // 關閉系統預設樣式
                }
                else if (c.HasChildren)
                {
                    // 如果是 GroupBox，就繼續往裡面找按鈕
                    ChangeButtonColor(c, newColor);
                }
            }
        }
        private void btnExportMenu_Click(object sender, EventArgs e)
        {
            // 如果還沒有玩過任何一局
            if (gameHistory.Count == 0)
            {
                MessageBox.Show("目前沒有任何遊戲紀錄可以輸出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 呼叫存檔對話框
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文字檔 (*.txt)|*.txt"; // 限制只能存成 txt 檔
            sfd.Title = "儲存遊戲紀錄";
            sfd.FileName = "撲克牌遊戲紀錄.txt"; // 預設檔名

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 使用 System.IO 將清單內的紀錄寫入文字檔
                    System.IO.File.WriteAllLines(sfd.FileName, gameHistory);
                    MessageBox.Show("紀錄成功輸出至：\n" + sfd.FileName, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("輸出檔案時發生錯誤：\n" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAboutMenu_Click(object sender, EventArgs e)
        {
            string aboutText = "五張撲克牌遊戲系統\n\n" +
                       "開發者：魏允鴻/1131417\n\n" +
                       "玩法說明：\n" +
                       "1. 輸入押注金額並點擊「押注」。\n" +
                       "2. 點擊「發牌」獲得五張手牌。\n" +
                       "3. 點選想換掉的牌 (牌面會朝下)，點擊「換牌」。\n" +
                       "4. 點擊「判斷牌型」結算獎金！" +
                       "5. 提供無滑鼠操作，設定好Tab順序與Enter鍵功能";
            MessageBox.Show(aboutText, "關於系統", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
