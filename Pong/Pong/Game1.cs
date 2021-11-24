// Autor: Vitor Martins de Sant'Anna
// Turma: 2003
// Numero: 34
// Professor: Yves
// Materia: Programação 02


#region Usings do projeto
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Pong
{
    /// <summary>
    /// Este é o principal método do jogo.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Costantes
        /// <summary>
        /// Multiplicador de velocidade da bola, este é o espaço da tela
        /// onde a bola vai viajar a cada segundo.
        /// </summary>
        const float BallSpeedMultiplicator = 0.5f;
        /// <summary>
        /// Velocidade da palheta do computador. Se a bola se move mais rápido
        /// para cima ou para baixo do que isso, a pá do computador não pode manter-se e,
        /// ocasiona na vitória do jogador.
        /// </summary>
        const float ComputerPaddleSpeed = 1.0f;

            #region Modos de jogo
        /// <summary>
        /// Modos de jogo.
        /// </summary>
        enum GameMode
        {
            Menu,
            Game,
            GameOver,
        } // enum GameMode

        /// <summary>
        /// Modo de jogo que estamos dentro atualmente. Fluxo de jogo muito simples.
        /// </summary>
        GameMode gameMode = GameMode.Menu;
            #endregion

            #region Estados do Teclado
        // Estados do teclado e dos game pads.
        GamePadState gamePad, gamePad2;
        KeyboardState keyboard;
            #endregion

            #region Gráficos
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
            #endregion

            #region Sons
        SoundEffect PongBallHit;
        SoundEffect PongBallLost;
        SoundEffect PongBallHitPaddle;
            #endregion
        #endregion

        #region Variaveis
            #region Inteiros (int)
        /// <summary>
        /// Atual menu selecionado.
        /// </summary>
        int currentMenuItem = 0;

        /// <summary>
        /// Delimita o numero de vidas dos jogadores.
        /// </summary>
        int leftPlayerLives;
        int rightPlayerLives;

        // Variáveis que guardarão as informações de Largura e Altura da tela respectivamente.
        int width, height;
            #endregion

            #region Decimais (float)
        // Essas variáveis definem a posição Y inicial das palhetas.
        float leftPaddlePosition = 0.5f;
        float rightPaddlePosition = 0.5f;
            #endregion

            #region Boleanos (bool)
        /// <summary>
        /// Se essa variável é falsa, o computador controlará a pá esquerda.
        /// Se for verdadeira o jogo será Multiplayer.
        /// </summary>
        bool multiplayer = false;

        /// <summary>
        /// Cima, baixo, começar e voltar dos botões do menu.
        /// </summary>
        bool remUpPressed = false;
        bool remDownPressed = false;
        bool remSpaceOrStartPressed = false;
        bool remEscOrBackPressed = false;

        bool gamePadUp = false;
        bool gamePadDown = false;
        bool gamePad2Up = false;
        bool gamePad2Down = false;

        bool gameStarted;
            #endregion

            #region Retângulos
        /// <summary>
        /// Recorta as imagens em retângulos que serão usados para fazer os elementos do jogo.
        /// </summary>
        static readonly Rectangle
            XnaPongLogoRect = new Rectangle(0, 0, 450, 74),
            MenuSinglePlayerRect = new Rectangle(127, 109, 207, 30),
            MenuMultiPlayerRect = new Rectangle(136, 169, 184, 31),
            MenuExitRect = new Rectangle(119, 222, 218, 29),
            GameBluePaddleRect = new Rectangle(14, 0, 13, 110),
            GameRedPaddleRect = new Rectangle(0, 0, 13, 110),
            Vidas = new Rectangle(0, 128, 22, 23),
            GameBallRect = new Rectangle(0, 110, 18, 18),
            GameBlueWonRect = new Rectangle(252, 138, 194, 35),
            GameRedWonRect = new Rectangle(1, 74, 271, 35),
            VitorMartins = new Rectangle(320, 198, 130, 20);
            #endregion

            #region Vetores
        // Guarda a posição da bola, e define a posição inicial desta.
        Vector2 ballPosition = new Vector2(0.5f, 0.5f);

        // Esta variável que é responsável por fazer a bola mudar de ângulo em sua tragetória.
        Vector2 ballSpeedVector = new Vector2(0, 0);

        // Converte o tamanho da bola em pixels para o equivalente ao tamanho da tela (0 - 1) em float.
        Vector2 ballSize = new Vector2(GameBallRect.Width / 800.0f, GameBallRect.Height / 600.0f);
            #endregion

            #region Texturas
        Texture2D backgroundTexture;
        Texture2D menuTexture; 
        Texture2D gameTexture;
            #endregion
        #endregion

        #region Construtor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialize
        protected override void Initialize()
        {
            // Pega a infomação da largura e altura da tela.
            width = graphics.GraphicsDevice.Viewport.Width;
            height = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();

            // Deixa o mause invisível.
            this.IsMouseVisible = false;

        }
        #endregion

        #region LoadContent
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carrega as imagens do jogo.
            backgroundTexture = Content.Load<Texture2D>("PongBackground");
            menuTexture = Content.Load<Texture2D>("PongMenu");
            gameTexture = Content.Load<Texture2D>("PongGame");

            PongBallHit = Content.Load<SoundEffect>("PongBallHit");
            PongBallLost = Content.Load<SoundEffect>("PongBallLost");
            PongBallHitPaddle = Content.Load<SoundEffect>("PongBallHitPaddle");

        }
        #endregion
        
        #region UnloadContent
        protected override void UnloadContent()
        {
        }
        #endregion
        
        #region Update
        protected override void Update(GameTime gameTime)
        {
            #region Fechar o Jogo
            // Permite fechar o jogo.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            #endregion

            #region Estados do Teclado e GamePads

            remUpPressed = false;
			remDownPressed = false;
            remSpaceOrStartPressed = false;
			remEscOrBackPressed = false;

            gamePadUp = false;
			gamePadDown = false;
			gamePad2Up = false;
			gamePad2Down = false;

            remUpPressed =
                gamePad.DPad.Up == ButtonState.Pressed ||
                gamePad.ThumbSticks.Left.Y > 0.5f ||
                keyboard.IsKeyDown(Keys.Up);
            remDownPressed =
                gamePad.DPad.Down == ButtonState.Pressed ||
                gamePad.ThumbSticks.Left.Y < -0.5f ||
                keyboard.IsKeyDown(Keys.Down);
            remSpaceOrStartPressed =
                gamePad.Buttons.Start == ButtonState.Pressed ||
                gamePad.Buttons.A == ButtonState.Pressed ||
                keyboard.IsKeyDown(Keys.LeftControl) ||
                keyboard.IsKeyDown(Keys.RightControl) ||
                keyboard.IsKeyDown(Keys.Space) ||
                keyboard.IsKeyDown(Keys.Enter);
            remEscOrBackPressed =
                gamePad.Buttons.Back == ButtonState.Pressed ||
                keyboard.IsKeyDown(Keys.Escape);

            // Obter os game pads atuais e os estados do teclado.
            gamePad = GamePad.GetState(PlayerIndex.One);
            gamePad2 = GamePad.GetState(PlayerIndex.Two);
            keyboard = Keyboard.GetState();

            gamePadUp = gamePad.DPad.Up == ButtonState.Pressed || gamePad.ThumbSticks.Left.Y > 0.5f;
            gamePadDown = gamePad.DPad.Down == ButtonState.Pressed || gamePad.ThumbSticks.Left.Y < -0.5f;
            gamePad2Up = gamePad2.DPad.Up == ButtonState.Pressed || gamePad2.ThumbSticks.Left.Y > 0.5f;
            gamePad2Down = gamePad2.DPad.Down == ButtonState.Pressed || gamePad2.ThumbSticks.Left.Y < -0.5f;
            #endregion

            #region Inputs
            // Mover do outro lado da tela a cada segundo.
            float moveFactorPerSecond = 0.5f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            // Move para cima ou para baixo se as teclas do teclado ou do game pad forem pressionadas.
            if (gamePadUp || keyboard.IsKeyDown(Keys.Up))
                rightPaddlePosition -= moveFactorPerSecond;
            if (gamePadDown || keyboard.IsKeyDown(Keys.Down))
                rightPaddlePosition += moveFactorPerSecond;
            // O segundo jogador é controlado pelo computador ou pelo jogador 2.
            if (multiplayer)
            {
                // Move para cima ou para baixo se as teclas do teclado ou do game pad forem pressionadas.
                if (gamePad2Up || keyboard.IsKeyDown(Keys.W))
                    leftPaddlePosition -= moveFactorPerSecond;
                if (gamePad2Down || keyboard.IsKeyDown(Keys.S))
                    leftPaddlePosition += moveFactorPerSecond;
            } // if
            else
            {
                // Apenas deixa que o computador siga a posição da bola.
                float computerChange = ComputerPaddleSpeed * moveFactorPerSecond;
                if (leftPaddlePosition > ballPosition.Y + computerChange)
                    leftPaddlePosition -= moveFactorPerSecond;// computerChange;
                else if (leftPaddlePosition < ballPosition.Y - computerChange)
                    leftPaddlePosition += moveFactorPerSecond; //computerChange;
            } // else
            #endregion

            #region Colisões
            // Faz as palhetas ficarem entre o limite inferior da tela, e o limite definido por mim .
            if (leftPaddlePosition < 0.07499891f)
                leftPaddlePosition = 0.07499891f;
            if (leftPaddlePosition > 1)
                leftPaddlePosition = 1;
            if (rightPaddlePosition < 0.07499891f)
                rightPaddlePosition = 0.07499891f;
            if (rightPaddlePosition > 1)
                rightPaddlePosition = 1;

            // Atualiza a posição da bola ao passar da borda.
            ballPosition += ballSpeedVector * moveFactorPerSecond * BallSpeedMultiplicator;

            // Verifica se a bola passou da borda inferior ou se passou do limite definido anteriormente.
            if (ballPosition.Y < 0.07499891f || ballPosition.Y > 1)
            {
                // Toca o som.
                PongBallHit.Play();
                // Inverte o alngulo que a bola estava indo.
                ballSpeedVector.Y = -ballSpeedVector.Y;
                // Move a bola de volta para o espaço da tela caso ela ultrapasse os limites da tela.
                if (ballPosition.Y < 0.07499891f)
                    ballPosition.Y = 0.07499891f;
                if (ballPosition.Y > 1)
                    ballPosition.Y = 1;
            } // if

            // Caixa de colisão da bola.
            BoundingBox ballBox = new BoundingBox(
                new Vector3(ballPosition.X - ballSize.X / 2, ballPosition.Y - ballSize.Y / 2, 0),
                new Vector3(ballPosition.X + ballSize.X / 2, ballPosition.Y + ballSize.Y / 2, 0));

            // Converte o tamanho das palhetas em pixel para o equivalente ao tamanho da tela em float.
            Vector2 paddleSize = 
                new Vector2(GameRedPaddleRect.Width / 800.0f, GameRedPaddleRect.Height / 600.0f);

            // Caixa de colisão da palheta da esquerda.
            BoundingBox leftPaddleBox = new BoundingBox(
                new Vector3(-paddleSize.X / 2, leftPaddlePosition - paddleSize.Y / 2, 0),
                new Vector3(+paddleSize.X / 2, leftPaddlePosition + paddleSize.Y / 2, 0));

            // Caixa de colisão da palheta da direita.
            BoundingBox rightPaddleBox = new BoundingBox(
                new Vector3(1 - paddleSize.X / 2, rightPaddlePosition - paddleSize.Y / 2, 0),
                new Vector3(1 + paddleSize.X / 2, rightPaddlePosition + paddleSize.Y / 2, 0));

            // Verifica se a bola bateu na palheta esquerda.
            if (ballBox.Intersects(leftPaddleBox))
            {
                // Toca o som de quando a bola toca nas palhetas.
                PongBallHitPaddle.Play();
                // Salta da palheta num ângulo contrário.
                ballSpeedVector.X = -ballSpeedVector.X;
                // Aumenta um pouco a velocidade.
                ballSpeedVector *= 1.05f;
                // Verifica se colidiu com as arestas da palheta.
                if (ballBox.Intersects(new BoundingBox(
                    new Vector3(leftPaddleBox.Min.X - 0.01f, leftPaddleBox.Min.Y - 0.01f, 0),
                    new Vector3(leftPaddleBox.Min.X + 0.01f, leftPaddleBox.Min.Y + 0.01f, 0))))
                {
                    // Salta da palheta com um ângulo maior para o outro jogador.
                    ballSpeedVector.Y = -2;
                }
                // Verifica se colidiu com a outra aresta da palheta.
                else if (ballBox.Intersects(new BoundingBox(
                    new Vector3(leftPaddleBox.Min.X - 0.01f, leftPaddleBox.Max.Y - 0.01f, 0),
                    new Vector3(leftPaddleBox.Min.X + 0.01f, leftPaddleBox.Max.Y + 0.01f, 0))))
                {
                    // Salta da palheta com um ângulo maior para o outro jogador.
                    ballSpeedVector.Y = +2;
                    // Se afasta da palheta.
                    ballPosition.X += moveFactorPerSecond * BallSpeedMultiplicator;
                }
            } // if

            // Verifica se a bola bateu na palheta direita.
            if (ballBox.Intersects(rightPaddleBox))
            {
                // Toca o som de quando a bola toca nas palhetas.
                PongBallHitPaddle.Play();
                // Salta da palheta.
                ballSpeedVector.X = -ballSpeedVector.X;
                // Aumenta um pouco a velocidade.
                ballSpeedVector *= 1.05f;
                // Verifica de colidiu com a aresta da palheta.
                if (ballBox.Intersects(new BoundingBox(
                    new Vector3(rightPaddleBox.Min.X - 0.01f, rightPaddleBox.Min.Y - 0.01f, 0),
                    new Vector3(rightPaddleBox.Min.X + 0.01f, rightPaddleBox.Min.Y + 0.01f, 0))))
                {
                    // Salta para o outro jogador com um ângulo maior.
                    ballSpeedVector.Y = -2;
                }
                // Verifica se colidiu com a outra aresta da palheta.
                else if (ballBox.Intersects(new BoundingBox(
                    new Vector3(rightPaddleBox.Min.X - 0.01f, rightPaddleBox.Max.Y - 0.01f, 0),
                    new Vector3(rightPaddleBox.Min.X + 0.01f, rightPaddleBox.Max.Y + 0.01f, 0))))
                {
                    // Salta para o outro jogador com um ângulo mais dificil e maior.
                    ballSpeedVector.Y = +2;
                    // Se afasta da palheta
                    ballPosition.X -= moveFactorPerSecond * BallSpeedMultiplicator;
                }
            } // if

            // Verifica de a bola passou dos limistes da esqueda e da direita da tela.
            if (ballPosition.X < -0.065f)
            {
                // Toca o som.
                PongBallLost.Play();
                // Diminui o numero de vidas.
                leftPlayerLives--;
                // Começa uma nova bola.
                StartNewBall();
            } // if
            else if (ballPosition.X > 1.065f)
            {
                // Toca o som.
                PongBallLost.Play();
                // Diminui o numero de vidas.
                rightPlayerLives--;
                // Começa uma nova bola.
                StartNewBall();
            } // if
            #endregion

            #region Definindo o modo de jogo atual
            // Se algum jogador não possui mais vidas, o outro jogador ganha o jogo.
            if (gameMode == GameMode.Game &&(leftPlayerLives == 0 || rightPlayerLives == 0))
            {
                gameMode = GameMode.GameOver;
                StopBall();
            } // if

            // Inicia a bola novamente se ela estiver parada e se estivermos no modo de jogo.
            if (ballSpeedVector.LengthSquared() == 0 &&
                gameMode == GameMode.Game)
                StartNewBall();



            // Se algum jogador não tiver mais vidas o outro jogador ganha.
            if (gameMode == GameMode.Game &&
                (leftPlayerLives == 0 ||
                rightPlayerLives == 0))
            {
                gameMode = GameMode.GameOver;
                StopBall();
            } // if (gameMode)

            base.Update(gameTime);
            #endregion
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, width, height), Color.WhiteSmoke);
            spriteBatch.End();

            if (gameMode == GameMode.Menu)
            {
                // Mostra o menu.
                RenderSprite(menuTexture,
                    400 - XnaPongLogoRect.Width / 2, 150, XnaPongLogoRect);
                RenderSprite(menuTexture,
                    400 - MenuSinglePlayerRect.Width / 2, 300, MenuSinglePlayerRect,
                    currentMenuItem == 0 ? Color.Black : Color.White);
                RenderSprite(menuTexture,
                    400 - MenuMultiPlayerRect.Width / 2, 350, MenuMultiPlayerRect,
                    currentMenuItem == 1 ? Color.Black : Color.White);
                RenderSprite(menuTexture,
                    400 - MenuExitRect.Width / 2, 400, MenuExitRect,
                    currentMenuItem == 2 ? Color.Black : Color.White);
                RenderSprite(menuTexture,
                    728 - VitorMartins.Width / 2, 570, VitorMartins, Color.LightGray);

                // Seleciona intens do menu.
                if ((keyboard.IsKeyDown(Keys.Down) || gamePadDown) && remDownPressed == false)
                {
                    currentMenuItem = (currentMenuItem + 1) % 3;
                    PongBallHit.Play();
                } // if (keyboard.IsKeyDown)
                else if ((keyboard.IsKeyDown(Keys.Up) || gamePadUp) && remUpPressed == false)
                {
                    currentMenuItem = (currentMenuItem + 2) % 3;
                    PongBallHit.Play();
                } // else if
                else if ((keyboard.IsKeyDown(Keys.Space) ||
                    keyboard.IsKeyDown(Keys.LeftControl) ||
                    keyboard.IsKeyDown(Keys.RightControl) ||
                    keyboard.IsKeyDown(Keys.Enter) ||
                    gamePad.Buttons.A == ButtonState.Pressed ||
                    gamePad.Buttons.Start == ButtonState.Pressed ||
                    // Ao pressionar Back ou Esc sai do jogo tanto no XBox, quanto no WIndows.
                    keyboard.IsKeyDown(Keys.Escape) ||
                    gamePad.Buttons.Back == ButtonState.Pressed) &&
                    remSpaceOrStartPressed == false &&
                    remEscOrBackPressed == false)
                {
                    // Toca o som ao selecionar algum item do menu.
                    PongBallLost.Play();
                    // Fecha o jogo.
                    if (currentMenuItem == 2 ||
                        keyboard.IsKeyDown(Keys.Escape) ||
                        gamePad.Buttons.Back == ButtonState.Pressed)
                    {
                        this.Exit();
                    } // if (currentMenuItem)
                    else
                    {
                        // Inicia o jogo.
                        gameMode = GameMode.Game;
                        leftPlayerLives = 3;
                        rightPlayerLives = 3;
                        leftPaddlePosition = 0.5f;
                        rightPaddlePosition = 0.5f;
                        StartNewBall();
                        // Defini se "Multiplayer" foi selecionado no menu.
                        // Caso contrário o computador que controlará a palheta esquerda.
                        multiplayer = currentMenuItem == 1;
                    } // else
                } // else if
            } // if (gameMode)
            else
            {
                // Chama o método ShowLives() e renderiza as vidas em seus devidos lugares.
                ShowLives();

                // Chama o método RenderBall() e renderiza a bola no meio.
                RenderBall();
                // Render both paddles
                RenderPaddles();

                // Se houver game over, o jogo mostra o vencedor.
                if (gameMode == GameMode.GameOver)
                {
                    if (leftPlayerLives == 0)
                        RenderSprite(menuTexture,
                            400 - GameBlueWonRect.Width / 2, 400, GameBlueWonRect);
                    else
                        RenderSprite(menuTexture,
                            400 - GameRedWonRect.Width / 2, 200, GameRedWonRect);

                    // A, espaço ou Enter retorna ao menu.
                    // Esc e Back fazem retornar ao menu.
                    // Verifica se as teclas estão liberadas antes de voltar ao menu.
                    if ((gamePad.Buttons.A == ButtonState.Pressed ||
                        keyboard.IsKeyDown(Keys.Space) ||
                        keyboard.IsKeyDown(Keys.Enter)) &&
                        remSpaceOrStartPressed == false)
                        gameMode = GameMode.Menu;
                } // if (gameMode)

                // Back e Esc sempre farão ir ao menu principal, tanto no GameMode.Game, ou no GameMode.GameOver.
                if ((gamePad.Buttons.Back == ButtonState.Pressed ||
                    keyboard.IsKeyDown(Keys.Escape)) &&
                    remEscOrBackPressed == false)
                {
                    gameMode = GameMode.Menu;
                    StopBall();
                } // if (gamePad.Buttons.Back)
            } // else

            DrawSprites();

            base.Draw(gameTime);
        }
        #endregion

        #region StartTest
        static void StartTest(TestDelegate testLoop)
        {
            using (testGame = new TestPongGame(testLoop))
            {
                testGame.Run();
            }// using
        }// StartTest(testLoop)
        #endregion

        #region SpriteToRender
        class SpriteToRender
        {
	        public Texture2D texture;
	        public Rectangle rect;
	        public Rectangle? sourceRect;
	        public Color color;
	        public SpriteToRender(Texture2D setTexture, Rectangle setRect, Rectangle? setSourceRect, Color setColor)
            {
               		texture = setTexture;
               		rect = setRect;
               		sourceRect = setSourceRect;
               		color = setColor;
            } // SpriteToRender(setTexture, setRect, setSourceRect)
        } // class SpriteToRender
        List<SpriteToRender> sprites = new List<SpriteToRender>();
        #endregion
        
        #region RenderSprite
        public void RenderSprite(Texture2D texture, Rectangle rect, Rectangle sourceRect)
        {
	        sprites.Add(new SpriteToRender(texture, rect, sourceRect,Color.White));
        } // RenderSprite(texture, rect, sourceRect)

        public void RenderSprite(Texture2D texture, Rectangle rect, Rectangle? sourceRect, Color color)
        {
        	sprites.Add(new SpriteToRender(texture, rect, sourceRect, color));
        } // RenderSprite(texture, rect, sourceRect, color)
        #endregion
        
        #region DrawSprites
        public void DrawSprites()
        {
            if (sprites.Count == 0)
                return;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (SpriteToRender sprite in sprites)
                spriteBatch.Draw(sprite.texture,
                  new Rectangle(
                  sprite.rect.X * width / 800,
                  sprite.rect.Y * height / 600,
                  sprite.rect.Width * width / 800,
                  sprite.rect.Height * height / 600),
                  sprite.sourceRect, sprite.color);
            spriteBatch.End();

            sprites.Clear();
        } // DrawSprites()
        #endregion

        #region StartGame
        public static void StartGame()
        {
            using (Game1 game = new Game1())
            {
                 game.Run();
            } // using
        } // StartGame()
        #endregion

        #region StartNewBall
        public void StartNewBall()
        {
            gameStarted = true;
            ballPosition = new Vector2(0.5f, 0.5f);
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int direction = rnd.Next(4);
            ballSpeedVector = direction == 0 ? new Vector2(1, 0.8f) : direction == 1 ? new Vector2(1, -0.8f) : direction == 2 ? new Vector2(-1, 0.8f) : new Vector2(-1, -0.8f);
        } // StartNewBall()
        #endregion

        #region StopBall
        public void StopBall()
        {
            ballSpeedVector = new Vector2(0, 0);
        } // StopBall()
        #endregion

        #region ShowLives
        public void ShowLives()
        {
            // Vidas do jogador da esquerda.
            RenderSprite(menuTexture, 2, 2, Vidas);
            for (int num = 0; num < leftPlayerLives; num++)
                RenderSprite(gameTexture, 2 + 6 + Vidas.Width * num - 2, 9, Vidas);

            // Vidas do jogador da direita.
            int rightX = 800 + Vidas.Width - Vidas.Width * 3 - 4;
            RenderSprite(menuTexture, rightX, 2, Vidas);
            for (int num = 0; num < rightPlayerLives; num++)
                RenderSprite(gameTexture, rightX + Vidas.Width - Vidas.Width * num - 2, 9, Vidas);

            RenderSprite(menuTexture,
                    400 - VitorMartins.Width / 2, 9, VitorMartins, Color.LightGray);

            /*// Vidas do jogador da esquerda.
            RenderSprite(menuTexture, 2, 2, Vidas);
            for (int num = 0; num < leftPlayerLives; num++)
                RenderSprite(gameTexture, 2 + Vidas.Width + Vidas.Width * num - 2, 9, Vidas);*/

            /*int rightX = 800 + Vidas.Width - Vidas.Width * 3 - 4;
            RenderSprite(menuTexture, rightX, 2, Vidas);
            for (int num = 0; num < rightPlayerLives; num++)
                RenderSprite(gameTexture, rightX + Vidas.Width - Vidas.Width * num - 2, 9, Vidas);*/
        }// ShowLives()
        #endregion

        #region Renderball
        public void RenderBall()
        {
            // Renderiza a bola.
            RenderSprite(gameTexture,
                (int)((0.05f + 0.9f * ballPosition.X) * 800) - GameBallRect.Width / 2,
                (int)((0.02f + 0.96f * ballPosition.Y) * 600) - GameBallRect.Height / 2,
                GameBallRect);
        } // RenderBall()
        #endregion

        #region RenderPaddles
        public void RenderPaddles()
        {
            // Palheta da esquerda.
            RenderSprite(gameTexture,
                (int)(0.05f * 800) - GameRedPaddleRect.Width / 2,
                (int)((-0.09f + 0.82f * leftPaddlePosition) * 600) + GameRedPaddleRect.Height / 2,
                GameRedPaddleRect);
            // Palheta da direita
            RenderSprite(gameTexture,
                (int)(0.95f * 800) - GameBluePaddleRect.Width / 2,
                (int)((-0.09f + 0.82f * rightPaddlePosition) * 600) + GameBluePaddleRect.Height / 2,
                GameBluePaddleRect);

        } // RenderPaddle(leftPaddle)
        #endregion
       
        #region RenderSprites
        public void RenderSprite(Texture2D texture, int x, int y, Rectangle? sourceRect, Color color)
        {
            ///<summary>
            ///Render sprite
            ///</summary>
            RenderSprite(texture, new Rectangle(x, y, sourceRect.Value.Width, sourceRect.Value.Height), sourceRect, color);
        } // RenderSprite(texture, x, y)
        public void RenderSprite(Texture2D texture, int x, int y, Rectangle? sourceRect)
        {
            ///<summary>
            ///Render sprite
            ///</summary>
            RenderSprite(texture, new Rectangle(x, y, sourceRect.Value.Width, sourceRect.Value.Height), sourceRect, Color.White);
        } // RenderSprite(texture, x, y)
        #endregion

        #region Teste Unitários
            #region TestBallCollisions
        public static void TestBallCollisions()
        {
            StartTest(
              delegate
              {
                  // Verifica se está no jogo, e se está no modo singleplayer.
                  testGame.gameMode = GameMode.Game;
                  testGame.multiplayer = false;
                  testGame.Window.Title = "Xna Pong - Press 1-5 to start collision tests";
                  // Diferentes tipos de colisões baseados na entrada do usuário (1, 2, 3, 4 e 5).
                  if (testGame.keyboard.IsKeyDown(Keys.D1))
                  {
                      // Primeiro teste, a bola apenas colide com a borda da tela.
                      testGame.ballPosition = new Vector2(0.6f, 0.9f);
                      testGame.ballSpeedVector = new Vector2(1, 1);
                  } // if
                  else if (testGame.keyboard.IsKeyDown(Keys.D2))
                  {
                      // O segundo teste, em linha reta em caso de colisão com a palheta direita.
                      testGame.ballPosition = new Vector2(0.85f, 0.6f);
                      testGame.ballSpeedVector = new Vector2(1, 1);
                      testGame.rightPaddlePosition = 0.7f;
                  } // if
                  else if (testGame.keyboard.IsKeyDown(Keys.D3))
                  {
                      // terceiro teste, em linha reta em caso de colisão com a palheta esquerda.
                      testGame.ballPosition = new Vector2(0.1f, 0.4f);
                      testGame.ballSpeedVector = new Vector2(-1, -0.5f);
                      testGame.leftPaddlePosition = 0.35f;
                  } // if
                  else if (testGame.keyboard.IsKeyDown(Keys.D4))
                  {
                      // Teste avançado para verificar se atingiu a aresta da palheta direita.
                      testGame.ballPosition = new Vector2(0.88f, 0.4f);
                      testGame.ballSpeedVector = new Vector2(1, -0.5f);
                      testGame.rightPaddlePosition = 0.35f;
                  } // if
                  else if (testGame.keyboard.IsKeyDown(Keys.D5))
                  {
                      // Teste avançado para verificar se atingiu a aresta da palheta esquerda.
                      testGame.ballPosition = new Vector2(0.1f, 0.4f);
                      testGame.ballSpeedVector = new Vector2(-1, -0.5f);
                      testGame.leftPaddlePosition = 0.25f;
                  } // if
                  // Mostra as vidas.
                  testGame.ShowLives();
                  // renderiza as bolas no centro da tela.
                  testGame.RenderBall();
                  // Renderiza as palhetas.
                  testGame.RenderPaddles();
              });
        } // TestBallCollisions ()
            #endregion   

            #region TestSinglePlayerGame
        public static void TestSinglePlayerGame()
        {
            StartTest(
                delegate
                {
                    // Mostrar vidas.
                    testGame.ShowLives();
                    // Bola no centro.
                    testGame.RenderBall();
                    // Renderiza as palhetas.
                    testGame.RenderPaddles();
                });
        } //TestSinglePlayerGame()
            #endregion

            #region TestPongGame
        class TestPongGame : Game1
        {
            TestDelegate testLoop;

            public TestPongGame(TestDelegate setTestLoop)
            {
                testLoop = setTestLoop;
            }// TestPongGame(setTestLoop)

            protected override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);
                testLoop();
                DrawSprites();
            }// Draw(Gametime)
        }//class TestPongGame

        static TestPongGame testGame;
            #endregion

            #region TestMenuSprites
        public static void TestMenuSprites()
        {
            StartTest(
                delegate
                {
                    testGame.RenderSprite(testGame.menuTexture, new Rectangle(400 - XnaPongLogoRect.Width
                    / 2, 150, XnaPongLogoRect.Width, XnaPongLogoRect.Height), XnaPongLogoRect);

                    testGame.RenderSprite(testGame.menuTexture, new Rectangle(400 - MenuSinglePlayerRect.Width
                    / 2, 300, MenuSinglePlayerRect.Width, MenuSinglePlayerRect.Height), MenuSinglePlayerRect);

                    testGame.RenderSprite(testGame.menuTexture, new Rectangle(400 - MenuMultiPlayerRect.Width
                    / 2, 350, MenuMultiPlayerRect.Width, MenuMultiPlayerRect.Height), MenuMultiPlayerRect, Color.Gold);

                    testGame.RenderSprite(testGame.menuTexture, new Rectangle(400 - MenuExitRect.Width
                    / 2, 400, MenuExitRect.Width, MenuExitRect.Height), MenuExitRect);
                });
        }
        delegate void TestDelegate();
            #endregion
        #endregion
    }
}