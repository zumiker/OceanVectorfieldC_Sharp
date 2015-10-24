using System;

using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.Shapes;
using Slb.Ocean.Petrel.DomainObject;
using Slb.Ocean.Geometry;
using System.Collections.Generic;
using Slb.Ocean.Basics;

namespace VectorField
{
    /// <summary>
    /// This class contains all the methods and subclasses of the CreateVF.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class CreateVF : Workstep<CreateVF.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>
        protected override CreateVF.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
        {
            return new Arguments(dataSourceManager);
        }
        /// <summary>
        /// Copies the Arguments instance.
        /// </summary>
        /// <param name="fromArgumentPackage">the source Arguments instance</param>
        /// <param name="toArgumentPackage">the target Arguments instance</param>
        protected override void CopyArgumentPackageCore(Arguments fromArgumentPackage, Arguments toArgumentPackage)
        {
            DescribedArgumentsHelper.Copy(fromArgumentPackage, toArgumentPackage);
        }

        /// <summary>
        /// Gets the unique identifier for this Workstep.
        /// </summary>
        protected override string UniqueIdCore
        {
            get
            {
                return "b11fdb78-6d57-460a-b086-2a7d02e0a5b6";
            }
        }
        #endregion

        #region IExecutorSource Members and Executor class

        /// <summary>
        /// Creates the Executor instance for this workstep. This class will do the work of the Workstep.
        /// </summary>
        /// <param name="argumentPackage">the argumentpackage to pass to the Executor</param>
        /// <param name="workflowRuntimeContext">the context to pass to the Executor</param>
        /// <returns>The Executor instance.</returns>
        public Slb.Ocean.Petrel.Workflow.Executor GetExecutor(object argumentPackage, WorkflowRuntimeContext workflowRuntimeContext)
        {
            return new Executor(argumentPackage as Arguments, workflowRuntimeContext);
        }

        public class Executor : Slb.Ocean.Petrel.Workflow.Executor
        {
            Arguments arguments;
            WorkflowRuntimeContext context;

            public Executor(Arguments arguments, WorkflowRuntimeContext context)
            {
                this.arguments = arguments;
                this.context = context;
            }

            public override void ExecuteSimple()
            {
                using (ITransaction trans = DataManager.NewTransaction())//объект удалением которого будет  с#
                {
                    /*Project proj = PetrelProject.PrimaryProject;
                    trans.Lock(arguments.HeiField.Samples);
                    trans.Lock(arguments.AzField.Samples);
                    trans.Lock(proj);
                    Index2 sij;
                    sij = arguments.HeiField.SpatialLattice.OriginalLattice.Single..Corners.SizeIJ;
                    int a = sij.I, b = sij.J;
                    List<RegularHeightFieldSample> samples;
                    samples = new List<RegularHeightFieldSample>();
                    for (int i = 0; i < a; i++)
                    {
                        for (int j = 0; j < b; j++)
                        {
                           samples.Add(new RegularHeightFieldSample(i, j, 0));
                        }
                    }
                  /*  int asas = 5;
                    foreach (RegularHeightFieldSample sample in arguments.HeiField.Samples)там уже есть значенияудаля
                        
                        samples.Insert(asas,sample);
                    }
                           /*}
                    }*/
                   // arguments.AzField.Samples 
                   /* Collection col = proj.CreateCollection("Surfaces");
                    int numi = 100, numj = 100;
                    double rotation = 0, spacingX = 25, spacingY = 50;
                    Point2 origin = new Point2(458000, 6780000);
                    LatticeInfo li = new LatticeInfo(origin.X, origin.Y, spacingX, spacingY, rotation, true, numi, numj, false, 0, 0, numi, numj);
                    RegularHeightFieldSurface surfa;
                    surfa = col.CreateRegularHeightFieldSurface("Test", new SpatialLatticeInfo(li, PetrelProject.PrimaryProject.GetCoordinateReferenceSystem()));
                    List<RegularHeightFieldSample> samples;
                    samples = new List<RegularHeightFieldSample>();
                    for (int j = 0; j < numj; j++)
                    {
                        for (int i = 0; i < numi; i++)
                        {
                            // give a sinusoidal shape to the Surface in both directions
                            double val = -1800 + 25*(1 - Math.Cos(i * 2 * Math.PI / numi))*(1 - Math.Cos(j * 2 * Math.PI / numj));
                            double val2 = 25 * (1 - Math.Cos(i * 2 * Math.PI / numi)) * (1 - Math.Cos(j * 2 * Math.PI / numj));
                            samples.Add(new RegularHeightFieldSample(i, j, val));
                            // direct access: surf[i, j] = val;
                        }
                    }*/
                    //surfa.Samples = samples;
                    //surfa.Name = "New Surface";
                    //trans.Commit();

                }
            }
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for CreateVF.
        /// Each public property is an argument in the package.  The name, type and
        /// input/output role are taken from the property and modified by any
        /// attributes applied.
        /// </summary>
        public class Arguments : DescribedArgumentsByReflection
        {
            public Arguments()
                : this(DataManager.DataSourceManager)
            {                
            }

            public Arguments(IDataSourceManager dataSourceManager)
            {
            }

            private Slb.Ocean.Petrel.DomainObject.Shapes.RegularHeightFieldSurface heiField;
            private Slb.Ocean.Petrel.DomainObject.Shapes.RegularHeightFieldSurface azField;
            private int step;
            private CustomVectorField vectorField;

            [Description("HeiField", "field1")]
            public Slb.Ocean.Petrel.DomainObject.Shapes.RegularHeightFieldSurface HeiField
            {
                internal get { return this.heiField; }
                set { this.heiField = value; }
            }

            [Description("AzField", "field2")]
            public Slb.Ocean.Petrel.DomainObject.Shapes.RegularHeightFieldSurface AzField
            {
                internal get { return this.azField; }
                set { this.azField = value; }
            }

            [Description("Step", "Шаг сетки")]
            public int Step
            {
                internal get { return this.step; }
                set { this.step = value; }
            }

            [Description("VectorField", "Векторное поле")]
            public CustomVectorField VectorField
            {
                get { return this.vectorField; }
                internal set { this.vectorField = value; }
            }


        }
    
        #region IAppearance Members
        public event EventHandler<TextChangedEventArgs> TextChanged;
        protected void RaiseTextChanged()
        {
            if (this.TextChanged != null)
                this.TextChanged(this, new TextChangedEventArgs(this));
        }

        public string Text
        {
            get { return Description.Name; }
            private set 
            {
                // TODO: implement set
                this.RaiseTextChanged();
            }
        }

        public event EventHandler<ImageChangedEventArgs> ImageChanged;
        protected void RaiseImageChanged()
        {
            if (this.ImageChanged != null)
                this.ImageChanged(this, new ImageChangedEventArgs(this));
        }

        public System.Drawing.Bitmap Image
        {
            get { return PetrelImages.Modules; }
            private set 
            {
                // TODO: implement set
                this.RaiseImageChanged();
            }
        }
        #endregion

        #region IDescriptionSource Members

        /// <summary>
        /// Gets the description of the CreateVF
        /// </summary>
        public IDescription Description
        {
            get { return CreateVFDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the CreateVF.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class CreateVFDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private  static CreateVFDescription instance = new CreateVFDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static CreateVFDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of CreateVF
            /// </summary>
            public string Name
            {
                get { return "CreateVF"; }
            }
            /// <summary>
            /// Gets the short description of CreateVF
            /// </summary>
            public string ShortDescription
            {
                get { return "век"; }
            }
            /// <summary>
            /// Gets the detailed description of CreateVF
            /// </summary>
            public string Description
            {
                get { return "Век"; }
            }

            #endregion
        }
        #endregion

        public class UIFactory : WorkflowEditorUIFactory
        {
            /// <summary>
            /// This method creates the dialog UI for the given workstep, arguments
            /// and context.
            /// </summary>
            /// <param name="workstep">the workstep instance</param>
            /// <param name="argumentPackage">the arguments to pass to the UI</param>
            /// <param name="context">the underlying context in which the UI is being used</param>
            /// <returns>a Windows.Forms.Control to edit the argument package with</returns>
            protected override System.Windows.Forms.Control CreateDialogUICore(Workstep workstep, object argumentPackage, WorkflowContext context)
            {
                return new CreateVFUI((CreateVF)workstep, (Arguments)argumentPackage, context);
            }
        }
    }
}