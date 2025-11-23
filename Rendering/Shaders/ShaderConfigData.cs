// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.Shaders.ShaderConfigData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering.Shaders;

public class ShaderConfigData
{
  public static ShaderConfigData.MatPropInt[] convertInts(string[] names, int[] vals)
  {
    ShaderConfigData.MatPropInt[] matPropIntArray = new ShaderConfigData.MatPropInt[names.Length];
    for (int index = 0; index < matPropIntArray.Length; ++index)
      matPropIntArray[index] = new ShaderConfigData.MatPropInt()
      {
        intName = names[index],
        intVal = vals[index]
      };
    return matPropIntArray;
  }

  public static ShaderConfigData.MatPropFloat[] convertFloats(string[] names, float[] vals)
  {
    ShaderConfigData.MatPropFloat[] matPropFloatArray = new ShaderConfigData.MatPropFloat[names.Length];
    for (int index = 0; index < matPropFloatArray.Length; ++index)
      matPropFloatArray[index] = new ShaderConfigData.MatPropFloat()
      {
        floatName = names[index],
        floatVal = vals[index]
      };
    return matPropFloatArray;
  }

  public static ShaderConfigData.MatPropMatrix[] convertMatrices(string[] names, Matrix4x4[] vals)
  {
    ShaderConfigData.MatPropMatrix[] matPropMatrixArray = new ShaderConfigData.MatPropMatrix[names.Length];
    for (int index = 0; index < matPropMatrixArray.Length; ++index)
      matPropMatrixArray[index] = new ShaderConfigData.MatPropMatrix()
      {
        matrixName = names[index],
        matrixVal = vals[index]
      };
    return matPropMatrixArray;
  }

  public static ShaderConfigData.MatPropVector[] convertVectors(string[] names, Vector4[] vals)
  {
    ShaderConfigData.MatPropVector[] matPropVectorArray = new ShaderConfigData.MatPropVector[names.Length];
    for (int index = 0; index < matPropVectorArray.Length; ++index)
      matPropVectorArray[index] = new ShaderConfigData.MatPropVector()
      {
        vectorName = names[index],
        vectorVal = vals[index]
      };
    return matPropVectorArray;
  }

  public static ShaderConfigData.MatPropTexture[] convertTextures(string[] names, Texture[] vals)
  {
    ShaderConfigData.MatPropTexture[] matPropTextureArray = new ShaderConfigData.MatPropTexture[names.Length];
    for (int index = 0; index < matPropTextureArray.Length; ++index)
      matPropTextureArray[index] = new ShaderConfigData.MatPropTexture()
      {
        textureName = names[index],
        textureVal = vals[index]
      };
    return matPropTextureArray;
  }

  public static string GetShaderPropertiesStringFromMaterial(Material mat, bool excludeMainTexData)
  {
    string stringFromMaterial = "";
    string[] propertyNames1 = mat.GetPropertyNames(MaterialPropertyType.Int);
    int[] numArray1 = new int[propertyNames1.Length];
    for (int index = 0; index < propertyNames1.Length; ++index)
    {
      numArray1[index] = mat.GetInteger(propertyNames1[index]);
      stringFromMaterial += numArray1[index].ToString();
    }
    string[] propertyNames2 = mat.GetPropertyNames(MaterialPropertyType.Float);
    float[] numArray2 = new float[propertyNames2.Length];
    for (int index = 0; index < propertyNames2.Length; ++index)
    {
      if (excludeMainTexData || !propertyNames2[index].Contains("_BaseMap"))
      {
        numArray2[index] = mat.GetFloat(propertyNames2[index]);
        stringFromMaterial += numArray2[index].ToString();
      }
    }
    string[] propertyNames3 = mat.GetPropertyNames(MaterialPropertyType.Matrix);
    Matrix4x4[] matrix4x4Array = new Matrix4x4[propertyNames3.Length];
    for (int index = 0; index < propertyNames3.Length; ++index)
    {
      matrix4x4Array[index] = mat.GetMatrix(propertyNames3[index]);
      stringFromMaterial += matrix4x4Array[index].ToString();
    }
    string[] propertyNames4 = mat.GetPropertyNames(MaterialPropertyType.Vector);
    Vector4[] vector4Array = new Vector4[propertyNames4.Length];
    for (int index = 0; index < propertyNames4.Length; ++index)
    {
      if (excludeMainTexData || !propertyNames4[index].Contains("_BaseMap"))
      {
        vector4Array[index] = mat.GetVector(propertyNames4[index]);
        stringFromMaterial += vector4Array[index].ToString();
      }
    }
    string[] propertyNames5 = mat.GetPropertyNames(MaterialPropertyType.Texture);
    Texture[] textureArray = new Texture[propertyNames5.Length];
    for (int index = 0; index < propertyNames5.Length; ++index)
    {
      if (!propertyNames5[index].Contains("_BaseMap"))
      {
        textureArray[index] = mat.GetTexture(propertyNames5[index]);
        if ((UnityEngine.Object) textureArray[index] != (UnityEngine.Object) null)
          stringFromMaterial += textureArray[index].ToString();
      }
    }
    return stringFromMaterial;
  }

  public static ShaderConfigData.ShaderConfig GetConfigDataFromMaterial(
    Material mat,
    bool includeMainTexData)
  {
    string[] propertyNames1 = mat.GetPropertyNames(MaterialPropertyType.Int);
    string[] intNames = propertyNames1;
    int[] intVals = new int[intNames.Length];
    bool flag1 = mat.IsKeywordEnabled("_WATER_EFFECT");
    bool flag2 = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
    bool flag3 = mat.IsKeywordEnabled("_UV_WAVE_WARP") | mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
    bool flag4 = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
    bool flag5 = mat.IsKeywordEnabled("_LIQUID_VOLUME") && !flag4;
    bool flag6 = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
    bool flag7 = mat.IsKeywordEnabled("_EMISSION") | flag6;
    bool flag8 = mat.IsKeywordEnabled("_REFLECTIONS");
    mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
    bool flag9 = mat.IsKeywordEnabled("_UV_SHIFT");
    for (int index = 0; index < intNames.Length; ++index)
    {
      intVals[index] = mat.GetInteger(propertyNames1[index]);
      if (!flag9 && (propertyNames1[index] == "_UvShiftSteps" || propertyNames1[index] == "_UvShiftOffset"))
        intVals[index] = 0;
    }
    string[] propertyNames2 = mat.GetPropertyNames(MaterialPropertyType.Float);
    string[] floatNames = propertyNames2;
    float[] floatVals = new float[floatNames.Length];
    for (int index = 0; index < propertyNames2.Length; ++index)
    {
      if (includeMainTexData || !propertyNames2[index].Contains("_BaseMap"))
        floatVals[index] = mat.GetFloat(propertyNames2[index]);
      if (!flag1 && propertyNames2[index] == "_HeightBasedWaterEffect" || !flag2 && propertyNames2[index] == "_RotateSpeed" || !flag3 && (propertyNames2[index] == "_WaveAmplitude" || propertyNames2[index] == "_WaveFrequency" || propertyNames2[index] == "_WaveScale") || !flag5 && (propertyNames2[index] == "_LiquidFill" || propertyNames2[index] == "_LiquidSwayX" || propertyNames2[index] == "_LiquidSwayY") || !flag6 && propertyNames2[index] == "_CrystalPower" || !flag7 && propertyNames2[index].StartsWith("_Emission") || !flag8 && (propertyNames2[index] == "_ReflectOpacity" || propertyNames2[index] == "_ReflectExposure" || propertyNames2[index] == "_ReflectRotate") || !flag9 && propertyNames2[index] == "_UvShiftRate")
        floatVals[index] = 0.0f;
    }
    string[] propertyNames3 = mat.GetPropertyNames(MaterialPropertyType.Matrix);
    string[] matrixNames = propertyNames3;
    Matrix4x4[] matrixVals = new Matrix4x4[matrixNames.Length];
    for (int index = 0; index < propertyNames3.Length; ++index)
      matrixVals[index] = mat.GetMatrix(propertyNames3[index]);
    string[] propertyNames4 = mat.GetPropertyNames(MaterialPropertyType.Vector);
    string[] vectorNames = propertyNames4;
    Vector4[] vectorVals = new Vector4[vectorNames.Length];
    for (int index = 0; index < propertyNames4.Length; ++index)
    {
      if (includeMainTexData || !propertyNames4[index].Contains("_BaseMap"))
        vectorVals[index] = mat.GetVector(propertyNames4[index]);
      if (!flag5 && (propertyNames4[index] == "_LiquidFillNormal" || propertyNames4[index] == "_LiquidSurfaceColor") || !flag4 && (propertyNames4[index] == "_LiquidPlanePosition" || propertyNames4[index] == "_LiquidPlaneNormal") || !flag6 && propertyNames4[index] == "_CrystalRimColor" || !flag7 && propertyNames4[index].StartsWith("_Emission") || !flag8 && (propertyNames4[index] == "_ReflectTint" || propertyNames4[index] == "_ReflectOffset" || propertyNames4[index] == "_ReflectScale"))
        vectorVals[index] = Vector4.zero;
    }
    string[] propertyNames5 = mat.GetPropertyNames(MaterialPropertyType.Texture);
    string[] textureNames = propertyNames5;
    Texture[] textureVals = new Texture[textureNames.Length];
    for (int index = 0; index < propertyNames5.Length; ++index)
    {
      if (!propertyNames5[index].Contains("_BaseMap"))
        textureVals[index] = mat.GetTexture(propertyNames5[index]);
    }
    return new ShaderConfigData.ShaderConfig(mat.shader.name, mat, intNames, intVals, floatNames, floatVals, matrixNames, matrixVals, vectorNames, vectorVals, textureNames, textureVals);
  }

  [Serializable]
  public struct ShaderConfig(
    string shadName,
    Material fMat,
    string[] intNames,
    int[] intVals,
    string[] floatNames,
    float[] floatVals,
    string[] matrixNames,
    Matrix4x4[] matrixVals,
    string[] vectorNames,
    Vector4[] vectorVals,
    string[] textureNames,
    Texture[] textureVals)
  {
    public string shaderName = shadName;
    public Material firstMat = fMat;
    public ShaderConfigData.MatPropInt[] ints = ShaderConfigData.convertInts(intNames, intVals);
    public ShaderConfigData.MatPropFloat[] floats = ShaderConfigData.convertFloats(floatNames, floatVals);
    public ShaderConfigData.MatPropMatrix[] matrices = ShaderConfigData.convertMatrices(matrixNames, matrixVals);
    public ShaderConfigData.MatPropVector[] vectors = ShaderConfigData.convertVectors(vectorNames, vectorVals);
    public ShaderConfigData.MatPropTexture[] textures = ShaderConfigData.convertTextures(textureNames, textureVals);
  }

  [Serializable]
  public struct MatPropInt
  {
    public string intName;
    public int intVal;
  }

  [Serializable]
  public struct MatPropFloat
  {
    public string floatName;
    public float floatVal;
  }

  [Serializable]
  public struct MatPropMatrix
  {
    public string matrixName;
    public Matrix4x4 matrixVal;
  }

  [Serializable]
  public struct MatPropVector
  {
    public string vectorName;
    public Vector4 vectorVal;
  }

  [Serializable]
  public struct MatPropTexture
  {
    public string textureName;
    public Texture textureVal;
  }

  [Serializable]
  public struct RenderersForShaderWithSameProperties
  {
    public MeshRenderer[] renderers;
  }
}
