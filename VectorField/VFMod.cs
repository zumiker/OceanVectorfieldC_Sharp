using System;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.UI.Tools;
using Slb.Ocean.Petrel.DomainObject;
using System.Windows.Forms;
using System.Drawing;

namespace VectorField
{
    /// <summary>
    /// This class will control the lifecycle of the Module.
    /// The order of the methods are the same as the calling order.
    /// </summary>
    /// 
    public class VFMod : IModule
    {

        public Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper wrapper;

        public VFMod()
        {
            //
            // TODO: Add constructor logic here
            //
        }

       
        #region IModule Members

        /// <summary>
        /// This method runs once in the Module life; when it loaded into the petrel.
        /// This method called first.
        /// </summary>
        /// 
        public void Initialize()
        {
            Type customVectorFieldType = typeof(CustomVectorField); 
            Type factoryType = typeof(IMapRenderer);
            CustomVectorFieldMapDisplay mapFactory = new CustomVectorFieldMapDisplay();
            CoreSystem.Services.AddService(customVectorFieldType, factoryType, mapFactory);
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the not UI related components.
        /// (eg: datasource, plugin)
        /// </summary>
        public void Integrate()
        {
            
            // TODO:  Add VFMod.Integrate implementation
            
            // Register CreateVF

            CreateVF createvfInstance = new CreateVF();
            PetrelSystem.WorkflowEditor.AddUIFactory<CreateVF.Arguments>(new CreateVF.UIFactory());
            PetrelSystem.WorkflowEditor.Add(createvfInstance);
            this.wrapper = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(createvfInstance);
            PetrelSystem.ProcessDiagram.Add(this.wrapper, "Plug-ins");
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the UI related components.
        /// (eg: settingspages, treeextensions)
        /// </summary>
        public void IntegratePresentation()
        {
            SimpleContextMenuHandler<CustomVectorField> cItem = new SimpleContextMenuHandler<CustomVectorField>("Редактировать исходные данные", PetrelImages.Modules, false, Editing);
            PetrelSystem.ToolService.AddContextMenuHandler(cItem);
        }

        /// <summary>
        /// This method called once in the life of the module; 
        /// right before the module is unloaded. 
        /// It is usually when the application is closing.
        /// </summary>
        public void Disintegrate()
        {
            // TODO:  Add VFMod.Disintegrate implementation
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO:  Add VFMod.Dispose implementation
        }

        #endregion

        public void Editing(object sender, ContextMenuClickedEventArgs<CustomVectorField> e)
        {
            CustomVectorField vf = e.ContextObject as CustomVectorField;
            CreateVF.Arguments a = new CreateVF.Arguments(); 

            using (ITransaction txn = DataManager.NewTransaction())
            {
                txn.Lock(vf);
                a.AzField = vf.Azimutfieldic;
                a.HeiField = vf.Heigtfieldic;
                a.Step = (int)vf.gridStep;
                a.VectorField = vf;
                CreateVFUI wnd = new CreateVFUI(wrapper.Workstep as CreateVF, a, null as WorkflowContext);
                Form das = new Form();
                das.Icon = Icon.FromHandle(PetrelImages.Modules.GetHicon());
                das.Text = "CreateVF";
                das.Width = 550;
                das.Height = 256;
                wnd.Parent = das;                
                wnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right))); 
                das.Show();
                txn.Commit();
            }
        }

    }
}