using LapixModules.debug;
using LapixModules.nsmi_mod;

namespace LapixModules
{
    public partial class Hub : Form
    {
        public Hub()
        {
            InitializeComponent();
            this.MethodTest();
        }

        private void MethodTest()
        {
            DebugOptions debugOptions = new();
            NSMIWrapper nSMIWrapper = new();

            debugOptions.GuaranteeFolder(AppContext.BaseDirectory + "TestFolder");
            nSMIWrapper.VerifyNvidiaSMI();

            MessageBox.Show("Current Power Limit: " + nSMIWrapper.GetPowerLimit() + "W" + "\n" + "TDP Modifiable?: " + nSMIWrapper.TDPModifiable());

        }
    }
}
