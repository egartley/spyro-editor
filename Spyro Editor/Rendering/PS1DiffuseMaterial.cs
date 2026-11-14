using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.WinUI;

namespace Spyro_Editor.Rendering
{
    public class PS1DiffuseMaterial : DiffuseMaterial
    {
        protected override MaterialCore OnCreateCore()
        {
            return new PS1DiffuseMaterialCore()
            {
                DiffuseColor = DiffuseColor,
                DiffuseMap = DiffuseMap,
                UVTransform = UVTransform,
                DiffuseMapSampler = DiffuseMapSampler,
                EnableUnLit = EnableUnLit,
                EnableFlatShading = EnableFlatShading
            };
        }
    }

    public class PS1DiffuseMaterialCore : DiffuseMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new DiffuseMaterialVariables(PS1EffectsManager.PS1DiffusePass, manager, technique, this);
        }
    }
}
