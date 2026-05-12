using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace ExCG
{
    public partial class FrmPrincipal : Form
    {
        private Bitmap canvas;
        private ObjModel modeloAtual;
        private bool arrastandoRotacao;
        private bool arrastandoTranslacao;
        private bool arrastandoRotacaoZ;
        private Point ultimoMouse;
        private float rotacaoX;
        private float rotacaoY;
        private float rotacaoZ;
        private float translacaoX;
        private float translacaoY;
        private float translacaoZ;
        private float escala;
        private string nomeArquivoAtual;

        public FrmPrincipal()
        {
            InitializeComponent();
            escala = 1.15f;
            rotacaoX = -18f;
            rotacaoY = 28f;
            rotacaoZ = 0f;
            translacaoX = 0f;
            translacaoY = 0f;
            translacaoZ = 0f;
            nomeArquivoAtual = string.Empty;
            ConfigurarInterface3D();
            AtualizarRenderizacao();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            AjustarLayout();
            pictureBox1.Focus();
        }

        private void ConfigurarInterface3D()
        {
            Text = "Visualizador OBJ 3D";
            ClientSize = new Size(1200, 760);
            MinimumSize = new Size(980, 640);
            BackColor = Color.FromArgb(240, 243, 247);

            pictureBox1.BackColor = Color.White;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.TabStop = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            btAbrirImg.Text = "Abrir OBJ";
            btAbrirImg.AutoSize = true;
            button1.Text = "Resetar Visão";
            button1.AutoSize = true;
            btLimpar.Text = "Limpar";
            btLimpar.AutoSize = true;
            checkBox1.Visible = false;

            Resize += FrmPrincipal_Resize;
        }

        private void FrmPrincipal_Resize(object sender, EventArgs e)
        {
            AjustarLayout();
            AtualizarRenderizacao();
        }

        private void AjustarLayout()
        {
            int margem = 12;
            int alturaBotoes = 34;
            int espacoTexto = 78;
            int larguraUtil = ClientSize.Width - (margem * 2);
            int alturaViewport = ClientSize.Height - (margem * 3) - alturaBotoes - espacoTexto;
            int topoBotoes = margem + alturaViewport + 12;

            if (alturaViewport < 200)
            {
                alturaViewport = 200;
            }

            pictureBox1.Location = new Point(margem, margem);
            pictureBox1.Size = new Size(larguraUtil, alturaViewport);

            btAbrirImg.Location = new Point(margem, topoBotoes);
            button1.Location = new Point(btAbrirImg.Right + 12, topoBotoes);
            btLimpar.Location = new Point(button1.Right + 12, topoBotoes);
        }

        private void btAbrirImg_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = string.Empty;
            openFileDialog.Filter = "Modelos OBJ (*.obj)|*.obj";
            openFileDialog.Title = "Selecione um arquivo .obj";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    modeloAtual = CarregarObj(openFileDialog.FileName);
                    nomeArquivoAtual = Path.GetFileName(openFileDialog.FileName);
                    ResetarVisaoPadrao();
                    AtualizarRenderizacao();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível abrir o arquivo OBJ.\n\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btLuminancia_Click(object sender, EventArgs e)
        {
            if (modeloAtual != null)
            {
                ResetarVisaoPadrao();
                AtualizarRenderizacao();
            }
        }

        private void btLimpar_Click(object sender, EventArgs e)
        {
            modeloAtual = null;
            nomeArquivoAtual = string.Empty;
            ResetarVisaoPadrao();

            if (canvas != null)
            {
                pictureBox1.Image = null;
                canvas.Dispose();
                canvas = null;
            }

            AtualizarRenderizacao();
        }

        private void ResetarVisaoPadrao()
        {
            rotacaoX = -18f;
            rotacaoY = 28f;
            rotacaoZ = 0f;
            translacaoX = 0f;
            translacaoY = 0f;
            translacaoZ = 0f;
            escala = 1.15f;
        }

        private ObjModel CarregarObj(string caminhoArquivo)
        {
            ObjModel modelo = new ObjModel();
            string[] linhas = File.ReadAllLines(caminhoArquivo);
            CultureInfo cultura = CultureInfo.InvariantCulture;
            int i = 0;

            while (i < linhas.Length)
            {
                string linha = linhas[i].Trim();

                if (linha.Length > 0 && !linha.StartsWith("#"))
                {
                    string[] partes = linha.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (partes.Length > 0)
                    {
                        if (partes[0] == "v" && partes.Length >= 4)
                        {
                            float x = float.Parse(partes[1], cultura);
                            float y = float.Parse(partes[2], cultura);
                            float z = float.Parse(partes[3], cultura);
                            modelo.Vertices.Add(new Vector3(x, y, z));
                        }

                        if (partes[0] == "f" && partes.Length >= 4)
                        {
                            ObjFace face = new ObjFace();
                            int j = 1;

                            while (j < partes.Length)
                            {
                                string[] referencia = partes[j].Split('/');
                                int indice = int.Parse(referencia[0], cultura);

                                if (indice > 0)
                                {
                                    indice = indice - 1;
                                }
                                else
                                {
                                    indice = modelo.Vertices.Count + indice;
                                }

                                if (indice >= 0 && indice < modelo.Vertices.Count)
                                {
                                    face.Indices.Add(indice);
                                }

                                j = j + 1;
                            }

                            if (face.Indices.Count >= 3)
                            {
                                modelo.Faces.Add(face);
                            }
                        }
                    }
                }

                i = i + 1;
            }

            if (modelo.Vertices.Count == 0)
            {
                throw new Exception("O arquivo OBJ não possui vértices válidos.");
            }

            CalcularLimitesModelo(modelo);
            return modelo;
        }

        private void CalcularLimitesModelo(ObjModel modelo)
        {
            Vector3 primeiro = modelo.Vertices[0];
            float minX = primeiro.X;
            float minY = primeiro.Y;
            float minZ = primeiro.Z;
            float maxX = primeiro.X;
            float maxY = primeiro.Y;
            float maxZ = primeiro.Z;
            int i = 1;

            while (i < modelo.Vertices.Count)
            {
                Vector3 v = modelo.Vertices[i];
                minX = Math.Min(minX, v.X);
                minY = Math.Min(minY, v.Y);
                minZ = Math.Min(minZ, v.Z);
                maxX = Math.Max(maxX, v.X);
                maxY = Math.Max(maxY, v.Y);
                maxZ = Math.Max(maxZ, v.Z);
                i = i + 1;
            }

            modelo.Centro = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
            modelo.DimensaoMaxima = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));

            if (modelo.DimensaoMaxima <= 0f)
            {
                modelo.DimensaoMaxima = 1f;
            }
        }

        private void AtualizarRenderizacao()
        {
            int largura = Math.Max(1, pictureBox1.Width);
            int altura = Math.Max(1, pictureBox1.Height);
            Bitmap novaImagem = new Bitmap(largura, altura);

            using (Graphics g = Graphics.FromImage(novaImagem))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush fundo = new LinearGradientBrush(new Rectangle(0, 0, largura, altura), Color.FromArgb(251, 252, 255), Color.FromArgb(224, 232, 244), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(fundo, 0, 0, largura, altura);
                }

                DesenharGrade(g, largura, altura);
                DesenharInstrucoes(g, largura, altura);

                if (modeloAtual != null)
                {
                    RenderizarModelo(g, largura, altura);
                }
                else
                {
                    string mensagem = "Abra um arquivo .obj para visualizar o modelo em 3D.";
                    SizeF tamanho = g.MeasureString(mensagem, Font);
                    float x = (largura - tamanho.Width) * 0.5f;
                    float y = (altura - tamanho.Height) * 0.5f;
                    g.DrawString(mensagem, Font, Brushes.DimGray, x, y);
                }
            }

            if (canvas != null)
            {
                canvas.Dispose();
            }

            canvas = novaImagem;
            pictureBox1.Image = canvas;
        }

        private void DesenharGrade(Graphics g, int largura, int altura)
        {
            using (Pen grade = new Pen(Color.FromArgb(32, 70, 90, 120), 1f))
            {
                int espacamento = 40;
                int x = 0;

                while (x <= largura)
                {
                    g.DrawLine(grade, x, 0, x, altura);
                    x = x + espacamento;
                }

                int y = 0;

                while (y <= altura)
                {
                    g.DrawLine(grade, 0, y, largura, y);
                    y = y + espacamento;
                }
            }
        }

        private void DesenharInstrucoes(Graphics g, int largura, int altura)
        {
            string linha1 = "Mouse esquerdo: rotaciona X/Y    |    Mouse direito: translada X/Y    |    Mouse do meio: rotaciona Z";
            string linha2 = "Scroll: escala    |    Ctrl + Scroll: translada Z    |    Botão Resetar Visão: volta ao padrão";
            string linha3 = modeloAtual != null
                ? string.Format("Arquivo: {0}    |    Vértices: {1}    |    Faces: {2}", nomeArquivoAtual, modeloAtual.Vertices.Count, modeloAtual.Faces.Count)
                : "Nenhum OBJ carregado";

            using (SolidBrush faixa = new SolidBrush(Color.FromArgb(225, 248, 250, 253)))
            using (SolidBrush texto = new SolidBrush(Color.FromArgb(45, 55, 72)))
            {
                g.FillRectangle(faixa, 12, altura - 66, largura - 24, 54);
                g.DrawString(linha1, Font, texto, 22, altura - 62);
                g.DrawString(linha2, Font, texto, 22, altura - 42);
                g.DrawString(linha3, Font, Brushes.Black, 22, altura - 22);
            }
        }

        private void RenderizarModelo(Graphics g, int largura, int altura)
        {
            List<Vector3> verticesTransformados = new List<Vector3>();
            List<PointF> verticesProjetados = new List<PointF>();
            List<FaceRenderizada> faces = new List<FaceRenderizada>();
            int i = 0;

            while (i < modeloAtual.Vertices.Count)
            {
                Vector3 transformado = TransformarVertice(modeloAtual.Vertices[i]);
                verticesTransformados.Add(transformado);
                verticesProjetados.Add(ProjetarVertice(transformado, largura, altura));
                i = i + 1;
            }

            int indiceFace = 0;

            while (indiceFace < modeloAtual.Faces.Count)
            {
                ObjFace face = modeloAtual.Faces[indiceFace];
                FaceRenderizada faceRenderizada = CriarFaceRenderizada(face, verticesTransformados, verticesProjetados);

                if (faceRenderizada != null)
                {
                    faces.Add(faceRenderizada);
                }

                indiceFace = indiceFace + 1;
            }

            faces.Sort(CompararProfundidadeFace);

            using (Pen contorno = new Pen(Color.FromArgb(190, 20, 20, 25), 1f))
            {
                int k = 0;

                while (k < faces.Count)
                {
                    FaceRenderizada face = faces[k];
                    using (SolidBrush preenchimento = new SolidBrush(face.Cor))
                    {
                        g.FillPolygon(preenchimento, face.Pontos);
                    }
                    g.DrawPolygon(contorno, face.Pontos);
                    k = k + 1;
                }
            }

            DesenharEixos(g, largura, altura);
        }

        private FaceRenderizada CriarFaceRenderizada(ObjFace face, List<Vector3> verticesTransformados, List<PointF> verticesProjetados)
        {
            FaceRenderizada resultado = null;

            if (face.Indices.Count >= 3)
            {
                PointF[] pontos = new PointF[face.Indices.Count];
                Vector3 soma = Vector3.Zero;
                int i = 0;

                while (i < face.Indices.Count)
                {
                    int indice = face.Indices[i];
                    pontos[i] = verticesProjetados[indice];
                    soma = soma + verticesTransformados[indice];
                    i = i + 1;
                }

                Vector3 v0 = verticesTransformados[face.Indices[0]];
                Vector3 v1 = verticesTransformados[face.Indices[1]];
                Vector3 v2 = verticesTransformados[face.Indices[2]];
                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
                float intensidade = 0.35f;

                if (normal.LengthSquared() > 0.00001f)
                {
                    normal = Vector3.Normalize(normal);
                    Vector3 luz = Vector3.Normalize(new Vector3(0.35f, 0.65f, 1.0f));
                    intensidade = 0.25f + (Math.Abs(Vector3.Dot(normal, luz)) * 0.75f);
                }

                int r = LimitarCor(45 + (int)(85f * intensidade));
                int g = LimitarCor(90 + (int)(105f * intensidade));
                int b = LimitarCor(140 + (int)(95f * intensidade));
                float profundidade = soma.Z / face.Indices.Count;
                Color cor = Color.FromArgb(220, r, g, b);
                resultado = new FaceRenderizada(pontos, profundidade, cor);
            }

            return resultado;
        }

        private int CompararProfundidadeFace(FaceRenderizada a, FaceRenderizada b)
        {
            int comparacao = a.ProfundidadeMedia.CompareTo(b.ProfundidadeMedia);
            return comparacao;
        }

        private int LimitarCor(int valor)
        {
            int resultado = valor;

            if (resultado < 0)
            {
                resultado = 0;
            }

            if (resultado > 255)
            {
                resultado = 255;
            }

            return resultado;
        }

        private void DesenharEixos(Graphics g, int largura, int altura)
        {
            Vector3 origem = TransformarPontoDeReferencia(Vector3.Zero);
            Vector3 eixoX = TransformarPontoDeReferencia(new Vector3(1.25f, 0f, 0f));
            Vector3 eixoY = TransformarPontoDeReferencia(new Vector3(0f, 1.25f, 0f));
            Vector3 eixoZ = TransformarPontoDeReferencia(new Vector3(0f, 0f, 1.25f));
            PointF p0 = ProjetarVertice(origem, largura, altura);
            PointF px = ProjetarVertice(eixoX, largura, altura);
            PointF py = ProjetarVertice(eixoY, largura, altura);
            PointF pz = ProjetarVertice(eixoZ, largura, altura);

            using (Pen penX = new Pen(Color.FromArgb(220, 215, 60, 60), 2f))
            using (Pen penY = new Pen(Color.FromArgb(220, 60, 170, 80), 2f))
            using (Pen penZ = new Pen(Color.FromArgb(220, 70, 120, 220), 2f))
            {
                g.DrawLine(penX, p0, px);
                g.DrawLine(penY, p0, py);
                g.DrawLine(penZ, p0, pz);
            }

            g.FillEllipse(Brushes.WhiteSmoke, p0.X - 4f, p0.Y - 4f, 8f, 8f);
            g.DrawEllipse(Pens.Black, p0.X - 4f, p0.Y - 4f, 8f, 8f);
            g.DrawString("X", Font, Brushes.Firebrick, px.X + 4f, px.Y + 2f);
            g.DrawString("Y", Font, Brushes.ForestGreen, py.X + 4f, py.Y + 2f);
            g.DrawString("Z", Font, Brushes.RoyalBlue, pz.X + 4f, pz.Y + 2f);
        }

        private Vector3 TransformarPontoDeReferencia(Vector3 ponto)
        {
            Vector3 rotacionado = AplicarRotacoes(ponto);
            Vector3 transladado = new Vector3(rotacionado.X + translacaoX, rotacionado.Y + translacaoY, rotacionado.Z + translacaoZ);
            return transladado;
        }

        private Vector3 TransformarVertice(Vector3 vertice)
        {
            Vector3 normalizado = (vertice - modeloAtual.Centro) / modeloAtual.DimensaoMaxima;
            Vector3 escalado = normalizado * escala;
            Vector3 rotacionado = AplicarRotacoes(escalado);
            Vector3 transladado = new Vector3(rotacionado.X + translacaoX, rotacionado.Y + translacaoY, rotacionado.Z + translacaoZ);
            return transladado;
        }

        private Vector3 AplicarRotacoes(Vector3 ponto)
        {
            float radX = GrausParaRadianos(rotacaoX);
            float radY = GrausParaRadianos(rotacaoY);
            float radZ = GrausParaRadianos(rotacaoZ);

            Vector3 rx = new Vector3(
                ponto.X,
                (ponto.Y * (float)Math.Cos(radX)) - (ponto.Z * (float)Math.Sin(radX)),
                (ponto.Y * (float)Math.Sin(radX)) + (ponto.Z * (float)Math.Cos(radX))
            );

            Vector3 ry = new Vector3(
                (rx.X * (float)Math.Cos(radY)) + (rx.Z * (float)Math.Sin(radY)),
                rx.Y,
                (-rx.X * (float)Math.Sin(radY)) + (rx.Z * (float)Math.Cos(radY))
            );

            Vector3 rz = new Vector3(
                (ry.X * (float)Math.Cos(radZ)) - (ry.Y * (float)Math.Sin(radZ)),
                (ry.X * (float)Math.Sin(radZ)) + (ry.Y * (float)Math.Cos(radZ)),
                ry.Z
            );

            return rz;
        }

        private PointF ProjetarVertice(Vector3 vertice, int largura, int altura)
        {
            float distanciaCamera = 4.4f;
            float profundidade = distanciaCamera - vertice.Z;

            if (profundidade < 0.25f)
            {
                profundidade = 0.25f;
            }

            float fator = (Math.Min(largura, altura) * 0.9f) / profundidade;
            float x = (largura * 0.5f) + (vertice.X * fator);
            float y = (altura * 0.5f) - (vertice.Y * fator);
            return new PointF(x, y);
        }

        private float GrausParaRadianos(float graus)
        {
            float radianos = graus * (float)Math.PI / 180f;
            return radianos;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            ultimoMouse = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    arrastandoRotacaoZ = true;
                }
                else
                {
                    arrastandoRotacao = true;
                }
            }

            if (e.Button == MouseButtons.Middle)
            {
                arrastandoRotacaoZ = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                arrastandoTranslacao = true;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int dx = e.X - ultimoMouse.X;
            int dy = e.Y - ultimoMouse.Y;

            if (arrastandoRotacao)
            {
                rotacaoY = rotacaoY + (dx * 0.7f);
                rotacaoX = rotacaoX + (dy * 0.7f);
                AtualizarRenderizacao();
            }

            if (arrastandoTranslacao)
            {
                translacaoX = translacaoX + (dx * 0.0045f);
                translacaoY = translacaoY - (dy * 0.0045f);
                AtualizarRenderizacao();
            }

            if (arrastandoRotacaoZ)
            {
                rotacaoZ = rotacaoZ + (dx * 0.7f);
                AtualizarRenderizacao();
            }

            ultimoMouse = e.Location;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastandoRotacao = false;
            arrastandoTranslacao = false;
            arrastandoRotacaoZ = false;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                translacaoZ = translacaoZ + (e.Delta / 120f) * 0.12f;
            }
            else
            {
                float fator = 1.0f + ((e.Delta / 120f) * 0.08f);
                escala = escala * fator;

                if (escala < 0.10f)
                {
                    escala = 0.10f;
                }

                if (escala > 12.0f)
                {
                    escala = 12.0f;
                }
            }

            AtualizarRenderizacao();
        }
    }

    internal sealed class ObjModel
    {
        public List<Vector3> Vertices;
        public List<ObjFace> Faces;
        public Vector3 Centro;
        public float DimensaoMaxima;

        public ObjModel()
        {
            Vertices = new List<Vector3>();
            Faces = new List<ObjFace>();
            Centro = Vector3.Zero;
            DimensaoMaxima = 1f;
        }
    }

    internal sealed class ObjFace
    {
        public List<int> Indices;

        public ObjFace()
        {
            Indices = new List<int>();
        }
    }

    internal sealed class FaceRenderizada
    {
        public PointF[] Pontos;
        public float ProfundidadeMedia;
        public Color Cor;

        public FaceRenderizada(PointF[] pontos, float profundidadeMedia, Color cor)
        {
            Pontos = pontos;
            ProfundidadeMedia = profundidadeMedia;
            Cor = cor;
        }
    }
}
