#if DEBUG_UI
using ImGuiNET;
#endif
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;


namespace ShevaEngine.Core
{
	/// <summary>
	/// Scene base class.
	/// </summary>
	public abstract class Scene :
#if DEBUG_UI
        IDebugUIPage, 
#endif
        IDisposable
    {        
        public List<Light> Lights { get; set; }        
        public string DebugUIPageName { get; private set; }
        protected List<IDisposable> Disposables;
        
        
		/// <summary>
        /// Constructor.
        /// </summary>
        public Scene()
        {
     		Lights = new List<Light>();
            Disposables = new List<IDisposable>();

#if DEBUG_UI
            ShevaGame.Instance.DebugUI.AddDebugPage(this);
#endif

            DebugUIPageName = $"Scene: {GetType().Name}";
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
                disposable?.Dispose();

            Disposables.Clear();
            Disposables = null;
        }
        
        /// <summary>
        /// Get visible objects method.
        /// </summary>
        public virtual void GetVisibleObjects(RenderingPipeline pipeline)
        {
			foreach (Light light in Lights)
				if (light.Enabled)
					pipeline.AddLight(light);												
        }

#if DEBUG_UI
        /// <summary>
        /// DebugUI.
        /// </summary>
        public virtual void DebugUI()
        {
            if (ImGui.TreeNode("Lights"))
            {
                for (int i = 0; i < Lights.Count; i++)
                {
                    if (ImGui.TreeNode($"Light {i}"))
                    {   
                        bool tempBool = Lights[i].Enabled;
			            ImGuiNET.ImGui.Checkbox("Enabled", ref tempBool);
			            Lights[i].Enabled = tempBool;

                        Vector3 temp = Lights[i].Color.ToVector3();
			            ImGuiNET.ImGui.ColorEdit3("Color", ref temp);			            

                        bool useSpecular = Lights[i].Color.A != 0;
                        ImGuiNET.ImGui.Checkbox("Use Specular", ref useSpecular);
			            
                        float tempSingle = Lights[i].Color.A / 255.0f;

                        if (useSpecular)              
                        {   
                            if (Lights[i].Color.A == 0)
                                tempSingle = 1.0f;

                            tempSingle = Math.Min(0.99f, 1.0f - tempSingle);                                                                                   

			                ImGuiNET.ImGui.SliderFloat("Specular factor", ref tempSingle, 0.0f, 0.99f);			                                                                    
                        }
                        
                        Lights[i].Color = new Color(temp.X, temp.Y, temp.Z, useSpecular ? 1.0f - tempSingle : 0.0f);
		                
		                // public Shadow Shadow { get; set; }

                        ImGui.TreePop();
                    }
                }

                ImGui.TreePop();
            }
        }
#endif
    }
}
