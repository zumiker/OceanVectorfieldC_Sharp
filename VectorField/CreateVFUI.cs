using System;
using System.Drawing;
using System.Windows.Forms;

using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel.DomainObject.Shapes;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.DomainObject;
using Slb.Ocean.Basics;
using Slb.Ocean.Geometry;
using Slb.Ocean.Petrel.UI.Tools;

namespace VectorField
{
    /// <summary>
    /// This class is the user interface which forms the focus for the capabilities offered by the process.  
    /// This often includes UI to set up arguments and interactively run a batch part expressed as a workstep.
    /// </summary>
    partial class CreateVFUI : UserControl
    {
        private CreateVF workstep;
        /// <summary>
        /// The argument package instance being edited by the UI.
        /// </summary>
        private CreateVF.Arguments args;
        /// <summary>
        /// Contains the actual underlaying context.
        /// </summary>
        private WorkflowContext context;
        private CreateVF.Arguments tmpargs = new CreateVF.Arguments();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVFUI"/> class.
        /// </summary>
        /// <param name="workstep">the workstep instance</param>
        /// <param name="args">the arguments</param>
        /// <param name="context">the underlying context in which this UI is being used</param>
        public CreateVFUI(CreateVF workstep, CreateVF.Arguments args, WorkflowContext context)
        {
            InitializeComponent();
            this.workstep = workstep;
            this.args = args;
            this.context = context;

            if (args.VectorField != null)
            {
                textBoxK.Text = args.VectorField.koeff.ToString();
                colorPicker1.Color = args.VectorField.arrowColor;
                textBoxN.Text = args.VectorField.partition.ToString();
                if (args.VectorField.haveGrid == true)
                {
                    comboBox1.SelectedItem = comboBox1.Items[0];
                }
                else
                {
                    comboBox1.SelectedItem = comboBox1.Items[1];
                }
                textBoxStep.Text = ((int)args.VectorField.gridStep).ToString();
            }
            workstep.CopyArgumentPackage(args, tmpargs);
            UpdateUiFormArgs();
        }

        private void dropTarget1_DragDrop(object sender, DragEventArgs e)
        {
            RegularHeightFieldSurface field = e.Data.GetData(typeof(object)) as RegularHeightFieldSurface;
            if (field == null)
            {
                PetrelLogger.ErrorBox("Объект не является поверхностью");
                return;
            }
            tmpargs.AzField = field;
            UpdateUiFormArgs();
        }

        private void dropTarget2_DragDrop(object sender, DragEventArgs e)
        {
            RegularHeightFieldSurface field = e.Data.GetData(typeof(object)) as RegularHeightFieldSurface;
            if (field == null)
            {
                PetrelLogger.ErrorBox("Объект не является поверхностью");
                return;
            }
            tmpargs.HeiField = field;
            UpdateUiFormArgs();
        }

        private void UpdateUiFormArgs()
        {
            if (tmpargs.AzField != null)
            {
                presentationBox1.Text = tmpargs.AzField.Description.Name;
                IImageInfoFactory imgSvc = CoreSystem.GetService<IImageInfoFactory>(tmpargs.AzField);
                presentationBox1.Image = imgSvc.GetImageInfo(tmpargs.AzField).TypeImage;
            }
            else
            {
                presentationBox1.Text = "";
                presentationBox1.Image = null;
            }
            if (tmpargs.HeiField != null)
            {
                presentationBox2.Text = tmpargs.HeiField.Description.Name;
                IImageInfoFactory imgSvc = CoreSystem.GetService<IImageInfoFactory>(tmpargs.HeiField);
                presentationBox2.Image = imgSvc.GetImageInfo(tmpargs.HeiField).TypeImage;
            }
            else
            {
                presentationBox2.Text = "";
                presentationBox2.Image = null;
            }
            btnOK.Enabled = !(tmpargs.AzField == null || tmpargs.HeiField == null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }        

        private void btnOK_Click(object sender, EventArgs e)
        {
            CustomVectorField NewVF;
            if (tmpargs.VectorField == null)
            {
                NewVF = new CustomVectorField();
            }
            else
            {
                NewVF = tmpargs.VectorField;
            }
            NewVF.koeff = double.Parse(textBoxK.Text);
            if ((NewVF.koeff > 1.0) || (NewVF.koeff <= 0.0))
            {
                PetrelLogger.ErrorBox("Коэффициент не может быть больше 1 или меньше 0!");
                return;
            }
            NewVF.gridStep = double.Parse(textBoxStep.Text);
            NewVF.partition = int.Parse(textBoxN.Text);
            NewVF.haveGrid = (comboBox1.SelectedItem.Text == "Да");
            NewVF.arrowColor = colorPicker1.Color; ;
            NewVF.originX = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.OriginX;
            NewVF.originY = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.OriginY;
            int sizeI = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.SizeI,
                sizeJ = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.SizeJ;
            double maxval = 0.0,
                   spacingI = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.SpacingI,
                   spacingJ = tmpargs.HeiField.SpatialLattice.OriginalLattice.Single.SpacingJ,
                   spaceSizeX = spacingI * (double)sizeI,
                   spaceSizeY = spacingJ * (double)sizeJ;
            NewVF.sizeX = (int)Math.Floor(spaceSizeX / (double)NewVF.gridStep);
            NewVF.sizeY = (int)Math.Floor(spaceSizeY / (double)NewVF.gridStep);
            NewVF.Azimutfieldic = tmpargs.AzField;
            NewVF.Heigtfieldic = tmpargs.HeiField;
            double[,] NewHeightSurface = new double[sizeI, sizeJ];
            double[,] NewAzimutSurface = new double[sizeI, sizeJ];
            foreach (RegularHeightFieldSample prop in tmpargs.HeiField.Samples)
            {
                NewHeightSurface[prop.I, prop.J] = prop.Value;
            }
            foreach (RegularHeightFieldSample prop in tmpargs.AzField.Samples)
            {
                NewAzimutSurface[prop.I, prop.J] = prop.Value;
            }
            double[, ,] VectorField = new double[NewVF.sizeX, NewVF.sizeY, 18];
            for (int i = 0; i < NewVF.sizeX; i++)
            {
                for (int j = 0; j < NewVF.sizeY; j++)
                {
                    //в каждой ячейке усредняем, берем из нее N значений значений
                    for (int k = 0; k < NewVF.partition; k++)
                    {
                        for (int l = 0; l < NewVF.partition; l++)
                        {
                            int x = (int)Math.Floor(((double)i * NewVF.gridStep + k * NewVF.gridStep / (double)NewVF.partition) / (double)spacingI),
                                y = (int)Math.Floor(((double)j * NewVF.gridStep + l * NewVF.gridStep / (double)NewVF.partition) / (double)spacingJ);
                            VectorField[i, j, 0] += NewHeightSurface[x, y];
                            VectorField[i, j, 1] += NewAzimutSurface[x, y];
                        }
                        VectorField[i, j, 0] /= (double)NewVF.partition * (double)NewVF.partition;
                        VectorField[i, j, 1] /= (double)NewVF.partition * (double)NewVF.partition;
                        if (maxval < VectorField[i, j, 0])
                        {
                            maxval = VectorField[i, j, 0];
                        }
                    }
                }
            }
            double tan, ctan;
            for (int i = 0; i < NewVF.sizeX; i++)
            {
                for (int j = 0; j < NewVF.sizeY; j++)
                {
                    VectorField[i, j, 0] /= maxval;
                    tan = Math.Tan(Math.PI / 2.0 - VectorField[i, j, 1]);
                    ctan = 1.0 / Math.Tan(Math.PI / 2.0 - VectorField[i, j, 1]);
                    if (tan > 1.0)
                    {
                        tan = 1.0;
                    }
                    else if (tan < -1.0)
                    {
                        tan = -1.0;
                    }
                    if (ctan > 1.0)
                    {
                        ctan = 1.0;
                    }
                    else if (ctan < -1.0)
                    {
                        ctan = -1.0;
                    }
                    VectorField[i, j, 2] = NewVF.originX + ((i + 0.5 * (1.0 - ctan * NewVF.koeff * VectorField[i, j, 0])) * NewVF.gridStep);
                    VectorField[i, j, 3] = NewVF.originY + ((j + 0.5 * (1.0 - tan * NewVF.koeff * VectorField[i, j, 0])) * NewVF.gridStep);
                    VectorField[i, j, 4] = NewVF.originX + ((i + 0.5 * (1.0 + ctan * NewVF.koeff * VectorField[i, j, 0])) * NewVF.gridStep);
                    VectorField[i, j, 5] = NewVF.originY + ((j + 0.5 * (1.0 + tan * NewVF.koeff * VectorField[i, j, 0])) * NewVF.gridStep);
                    float vx = (float)VectorField[i, j, 4] - (float)VectorField[i, j, 2];
                    float vy = (float)VectorField[i, j, 5] - (float)VectorField[i, j, 3];
                    float dist = (float)Math.Sqrt(vx * vx + vy * vy);
                    vx /= dist;
                    vy /= dist;
                    float h = 0.5f*dist;
                    float h1 = 0.35f*dist;
                    float w = h * (float)1/ (float)3.0;
                    float w1 = h1 * (float)1 / (float)10.0;
                    float ox = (float)VectorField[i, j, 4] - h * vx;
                    float oy = (float)VectorField[i, j, 5] - h * vy;
                    float ox1 = (float)VectorField[i, j, 4] - h1 * vx;
                    float oy1 = (float)VectorField[i, j, 5] - h1 * vy;
                    VectorField[i, j, 6] = ox + w * (VectorField[i, j, 3] - VectorField[i, j, 5])/dist;
                    VectorField[i, j, 7] = oy + w * (VectorField[i, j, 4] - VectorField[i, j, 2])/dist;
                    VectorField[i, j, 8] = ox - w * (VectorField[i, j, 3] - VectorField[i, j, 5])/dist;
                    VectorField[i, j, 9] = oy - w * (VectorField[i, j, 4] - VectorField[i, j, 2])/dist;
                    VectorField[i, j, 10] = ox1 - w1 * (VectorField[i, j, 3] - VectorField[i, j, 5]) / dist;
                    VectorField[i, j, 11] = oy1 - w1 * (VectorField[i, j, 4] - VectorField[i, j, 2]) / dist;
                    VectorField[i, j, 12] = ox1 + w1 * (VectorField[i, j, 3] - VectorField[i, j, 5]) / dist;
                    VectorField[i, j, 13] = oy1 + w1 * (VectorField[i, j, 4] - VectorField[i, j, 2]) / dist;
                    VectorField[i, j, 14] = (float)VectorField[i, j, 2] - w1 * (VectorField[i, j, 3] - VectorField[i, j, 5]) / dist;
                    VectorField[i, j, 15] = (float)VectorField[i, j, 3] - w1 * (VectorField[i, j, 4] - VectorField[i, j, 2]) / dist;
                    VectorField[i, j, 16] = (float)VectorField[i, j, 2] + w1 * (VectorField[i, j, 3] - VectorField[i, j, 5]) / dist;
                    VectorField[i, j, 17] = (float)VectorField[i, j, 3] + w1 * (VectorField[i, j, 4] - VectorField[i, j, 2]) / dist; 
                    

                }
            }
            NewVF.VectorField = VectorField;
            if (tmpargs.VectorField == null)
            {

                tmpargs.VectorField = NewVF;
                using (ITransaction txn = DataManager.NewTransaction())
                {
                    Project proj = PetrelProject.PrimaryProject;
                    txn.Lock(proj);
                    proj.Extensions.Add(tmpargs.VectorField);



                    txn.Commit();
                }
            }
            MapWindow map = PetrelProject.ToggleWindows.Add(WellKnownWindows.Map) as MapWindow;
            
            map.ShowObject(tmpargs.HeiField);
            map.ShowObject(tmpargs.AzField);
            map.ShowObject(NewVF);

           
            this.ParentForm.Close();
          
        }

        private void CreateVFUI_Load(object sender, EventArgs e)
        {
            if (args.VectorField == null)
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
            }
            UpdateUiFormArgs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}


