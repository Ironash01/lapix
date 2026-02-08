using LapixModules.debug;
using LapixModules.nsmi_mod;

namespace LapixModules
{
    public partial class Hub : Form
    {

        enum Modules
        {
            NvidiaSMIWrapper
        }
        public Hub()
        {
            InitializeComponent();
            this.ModuleLoaderUI();
            this.MethodTest();
        }

        private void ModuleLoaderUI()
        {
            foreach (Modules modules in Enum.GetValues(typeof(Modules)))
            {
                ModuleSelector.Items.Add(modules);
            }

            // Default to first module.
            ModuleSelector.SelectedIndex = 0;
        }



        private void MethodTest()
        {
            DebugOptions debugOptions = new();
            NSMIWrapper nSMIWrapper = new();

            debugOptions.GuaranteeFolder(AppContext.BaseDirectory + "TestFolder");
            nSMIWrapper.VerifyNvidiaSMI();
            nSMIWrapper.GetSupportedClocks();
            nSMIWrapper.GetSupportedMemoryClocks();

        }
    }
}
