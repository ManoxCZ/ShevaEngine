using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Materials manager.
	/// </summary>
	public class MaterialsManager
	{ 
		/// <summary>
		/// Update materials.
		/// </summary>		
		internal static Model UpdateMaterials(Model model)
		{
			foreach (ModelMesh mesh in model.Meshes)
				UpdateMaterials(mesh);
			
			return model;
		}

		/// <summary>
		/// Update materials.
		/// </summary>
		internal static void UpdateMaterials(ModelMesh mesh)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				UpdateVertexDeclarationTag(meshPart.VertexBuffer.VertexDeclaration);

				if (meshPart.Effect is BasicEffect basicEffect)
				{
					if (basicEffect.Texture == null)
					{
						// Vector3 temp = new Vector3(
						// 	(float)(basicEffect.DiffuseColor.X * (basicEffect.DiffuseColor.X * (basicEffect.DiffuseColor.X * 0.305306011 + 0.682171111) + 0.012522878)),
						// 	(float)(basicEffect.DiffuseColor.Y * (basicEffect.DiffuseColor.Y * (basicEffect.DiffuseColor.Y * 0.305306011 + 0.682171111) + 0.012522878)),
						// 	(float)(basicEffect.DiffuseColor.Z * (basicEffect.DiffuseColor.Z * (basicEffect.DiffuseColor.Z * 0.305306011 + 0.682171111) + 0.012522878)));

						Vector3 temp = new Vector3(
							(float)(0.585122381 * Math.Sqrt(basicEffect.DiffuseColor.X) + 0.783140355 * Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.X)) - 0.368262736 * Math.Sqrt(Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.X)))),
							(float)(0.585122381 * Math.Sqrt(basicEffect.DiffuseColor.Y) + 0.783140355 * Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.Y)) - 0.368262736 * Math.Sqrt(Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.Y)))),
							(float)(0.585122381 * Math.Sqrt(basicEffect.DiffuseColor.Z) + 0.783140355 * Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.Z)) - 0.368262736 * Math.Sqrt(Math.Sqrt(Math.Sqrt(basicEffect.DiffuseColor.Z)))));
						
						meshPart.Effect = new ColoredMaterial()
						{
							Animated = false,
							Transparent = basicEffect.Alpha != 1.0f,
							Color = Color.FromNonPremultiplied(new Vector4(temp, basicEffect.Alpha)),
							SpecularFactor = basicEffect.SpecularPower
						};
					}
					else
					{
						meshPart.Effect = new TexturedMaterial()
						{
							Animated = false,
							Transparent = basicEffect.Alpha != 1.0f,
							Color = Color.FromNonPremultiplied(new Vector4(basicEffect.DiffuseColor, basicEffect.Alpha)),
							SpecularFactor = basicEffect.SpecularPower,
							Texture = basicEffect.Texture
						};
					}
				}
				if (meshPart.Effect is SkinnedEffect skinnedEffect)
				{
					//if (skinnedEffect.Texture == null)
					{
						meshPart.Effect = new ColoredMaterial()
						{
							Animated = true,
							Bones = Material.BonesIdentity,
							Transparent = skinnedEffect.Alpha != 1.0f,
							Color = Color.FromNonPremultiplied(new Vector4(skinnedEffect.DiffuseColor, skinnedEffect.Alpha)),
							SpecularFactor = skinnedEffect.SpecularPower
						};
					}
					//else
					//{
					//	meshPart.Effect = new TexturedMaterial()
					//	{
					//		Animated = true,
					//		Bones = Material.BonesIdentity,
					//		Transparent = skinnedEffect.Alpha != 255,
					//		Color = Color.FromNonPremultiplied(new Vector4(skinnedEffect.DiffuseColor, skinnedEffect.Alpha)),
					//		SpecularFactor = skinnedEffect.SpecularPower,
					//		Texture = skinnedEffect.Texture
					//	};
					//}
				}
			}
		}

        /// <summary>
		/// Update vertex declaration tag.
		/// </summary>		
		public static string GetVertexDeclarationName(VertexDeclaration vertexDeclaration)
        {
            string name = string.Empty;

            foreach (VertexElement element in vertexDeclaration.GetVertexElements())
                name += element.VertexElementUsage.ToString()[0];

            name += vertexDeclaration.VertexStride.ToString();

            return name;
        }

        /// <summary>
        /// Update vertex declaration tag.
        /// </summary>		
        internal static void UpdateVertexDeclarationTag(VertexDeclaration vertexDeclaration)
        {
            vertexDeclaration.Name = GetVertexDeclarationName(vertexDeclaration);
        }
	}
}
