using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Visca;

namespace PDE.CameraViscaPlugin.EPI
{
    public partial class CameraVisca
    {
        #region Memory Commands Definition

        private readonly ViscaMemorySet _memorySetCmd;
        private readonly ViscaMemoryRecall _memoryRecallCmd;

        #endregion Memory Commands Definition

        #region Memory Commands Implementations

        public void MemorySet(byte preset) { _visca.EnqueueCommand(_memorySetCmd.UsePreset(preset)); }
        public void MemoryRecall(byte preset) { _visca.EnqueueCommand(_memoryRecallCmd.UsePreset(preset)); }

        #endregion Memory Commands Implementations
    }
}