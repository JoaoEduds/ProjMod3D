using System.Windows.Forms;

namespace ExCG
{
    public partial class FrmPrincipal : Form
    {
        private Image image;
        private Bitmap imgBitmap;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {

        }

        private void btAbrirImg_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.gif;*.bmp;*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        private void btLuminancia_Click(object sender, EventArgs e)
        {
            Bitmap imgDest = new Bitmap(image);
            imgBitmap = (Bitmap)image;
            Filtros.luminancia(imgBitmap, imgDest);
            pictureBox1.Image = imgDest;
        }

        private void btLimpar_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
    }
}
