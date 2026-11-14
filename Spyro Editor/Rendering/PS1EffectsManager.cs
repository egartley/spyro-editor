using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Shaders;
using System;
using System.IO;

namespace Spyro_Editor.Rendering
{
    public class PS1EffectsManager : DefaultEffectsManager
    {
        public const string PS1DiffusePass = "PS1Diffuse";

        public PS1EffectsManager()
        {
            ShaderDescription ps = new ShaderDescription("PSPS1Diffuse", ShaderStage.Pixel, new ShaderReflector(),
                File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Shaders", "psPS1Diffuse.cso")));
            IRenderTechnique mesh = GetTechnique(DefaultRenderTechniqueNames.Mesh);
            ShaderPassDescription pass = new ShaderPassDescription(PS1DiffusePass)
            {
                ShaderList = [DefaultVSShaderDescriptions.VSMeshDefault, ps],
                BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual,
                InputLayoutDescription = mesh.Description.InputLayoutDescription
            };
            mesh.Description.PassDescriptions = [.. mesh.Description.PassDescriptions, pass];
            mesh.AddPass(pass);
        }
    }
}
