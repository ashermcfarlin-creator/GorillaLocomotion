// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.GTMicWrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class GTMicWrapper : MicWrapper
{
  private bool _allowPitchAdjustment;
  private float _pitchAdjustment = 1f;
  private bool _allowVolumeAdjustment;
  private float _volumeAdjustment = 1f;
  private static readonly int MaxFrameLength = 16000;
  private readonly float[] InFifo = new float[GTMicWrapper.MaxFrameLength];
  private readonly float[] OutFifo = new float[GTMicWrapper.MaxFrameLength];
  private readonly float[] FfTworksp = new float[2 * GTMicWrapper.MaxFrameLength];
  private readonly float[] LastPhase = new float[GTMicWrapper.MaxFrameLength / 2 + 1];
  private readonly float[] SumPhase = new float[GTMicWrapper.MaxFrameLength / 2 + 1];
  private readonly float[] OutputAccum = new float[2 * GTMicWrapper.MaxFrameLength];
  private readonly float[] AnaFreq = new float[GTMicWrapper.MaxFrameLength];
  private readonly float[] AnaMagn = new float[GTMicWrapper.MaxFrameLength];
  private readonly float[] SynFreq = new float[GTMicWrapper.MaxFrameLength];
  private readonly float[] SynMagn = new float[GTMicWrapper.MaxFrameLength];
  private long _gRover;

  public GTMicWrapper(
    string device,
    int suggestedFrequency,
    bool allowPitchAdjustment,
    float pitchAdjustment,
    bool allowVolumeAdjustment,
    float volumeAdjustment,
    Photon.Voice.ILogger logger)
    : base(device, suggestedFrequency, logger)
  {
    this.UpdatePitchAdjustment(allowPitchAdjustment, pitchAdjustment);
    this.UpdateVolumeAdjustment(allowVolumeAdjustment, volumeAdjustment);
  }

  public void UpdateWrapper(
    bool allowPitchAdjustment,
    float pitchAdjustment,
    bool allowVolumeAdjustment,
    float volumeAdjustment)
  {
    this.UpdatePitchAdjustment(allowPitchAdjustment, pitchAdjustment);
    this.UpdateVolumeAdjustment(allowVolumeAdjustment, volumeAdjustment);
  }

  public void UpdatePitchAdjustment(bool allow, float pitchAdjustment)
  {
    this._allowPitchAdjustment = allow;
    this._pitchAdjustment = pitchAdjustment;
  }

  public void UpdateVolumeAdjustment(bool allow, float volumeAdjustment)
  {
    this._allowVolumeAdjustment = allow;
    this._volumeAdjustment = volumeAdjustment;
  }

  public override bool Read(float[] buffer)
  {
    if (this.Error != null)
      return false;
    int position = UnityMicrophone.GetPosition(this.device);
    if (position < this.micPrevPos)
      ++this.micLoopCnt;
    this.micPrevPos = position;
    int num1 = this.micLoopCnt * this.mic.samples + position;
    if (this.mic.channels == 0)
    {
      this.Error = "Number of channels is 0 in Read()";
      this.logger.LogError("[PV] MicWrapper: " + this.Error);
      return false;
    }
    int numSampsToProcess = buffer.Length / this.mic.channels;
    int num2 = this.readAbsPos + numSampsToProcess;
    if (num2 >= num1)
      return false;
    this.mic.GetData(buffer, this.readAbsPos % this.mic.samples);
    this.readAbsPos = num2;
    float num3 = Mathf.Clamp(this._pitchAdjustment, 0.5f, 2f);
    if (this._allowPitchAdjustment && !Mathf.Approximately(num3, 1f))
      this.PitchShift(num3, (long) numSampsToProcess, (float) this.SamplingRate, buffer);
    if (this._allowVolumeAdjustment && !Mathf.Approximately(this._volumeAdjustment, 1f))
    {
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = Mathf.Clamp(buffer[index] * this._volumeAdjustment, float.MinValue, float.MaxValue);
    }
    return true;
  }

  private void PitchShift(
    float pitchShift,
    long numSampsToProcess,
    float sampleRate,
    float[] indata)
  {
    this.PitchShift(pitchShift, numSampsToProcess, 2048L /*0x0800*/, 10L, sampleRate, indata);
  }

  public void PitchShift(
    float pitchShift,
    long numSampsToProcess,
    long fftFrameSize,
    long osamp,
    float sampleRate,
    float[] indata)
  {
    float[] numArray = indata;
    long num1 = fftFrameSize / 2L;
    long num2 = fftFrameSize / osamp;
    double num3 = (double) sampleRate / (double) fftFrameSize;
    double num4 = 2.0 * Math.PI * (double) num2 / (double) fftFrameSize;
    long num5 = fftFrameSize - num2;
    if (this._gRover == 0L)
      this._gRover = num5;
    for (long index1 = 0; index1 < numSampsToProcess; ++index1)
    {
      this.InFifo[this._gRover] = indata[index1];
      numArray[index1] = this.OutFifo[this._gRover - num5];
      ++this._gRover;
      if (this._gRover >= fftFrameSize)
      {
        this._gRover = num5;
        for (long index2 = 0; index2 < fftFrameSize; ++index2)
        {
          double num6 = -0.5 * Math.Cos(2.0 * Math.PI * (double) index2 / (double) fftFrameSize) + 0.5;
          this.FfTworksp[2L * index2] = this.InFifo[index2] * (float) num6;
          this.FfTworksp[2L * index2 + 1L] = 0.0f;
        }
        this.ShortTimeFourierTransform(this.FfTworksp, fftFrameSize, -1L);
        for (long index3 = 0; index3 <= num1; ++index3)
        {
          double x = (double) this.FfTworksp[2L * index3];
          double y = (double) this.FfTworksp[2L * index3 + 1L];
          double num7 = 2.0 * Math.Sqrt(x * x + y * y);
          double num8 = Math.Atan2(y, x);
          double num9 = num8 - (double) this.LastPhase[index3];
          this.LastPhase[index3] = (float) num8;
          double num10 = num9 - (double) index3 * num4;
          long num11 = (long) (num10 / Math.PI);
          long num12 = num11 < 0L ? num11 - (num11 & 1L) : num11 + (num11 & 1L);
          double num13 = num10 - Math.PI * (double) num12;
          double num14 = (double) osamp * num13 / (2.0 * Math.PI);
          double num15 = (double) index3 * num3 + num14 * num3;
          this.AnaMagn[index3] = (float) num7;
          this.AnaFreq[index3] = (float) num15;
        }
        for (int index4 = 0; (long) index4 < fftFrameSize; ++index4)
        {
          this.SynMagn[index4] = 0.0f;
          this.SynFreq[index4] = 0.0f;
        }
        for (long index5 = 0; index5 <= num1; ++index5)
        {
          long index6 = (long) ((double) index5 * (double) pitchShift);
          if (index6 <= num1)
          {
            this.SynMagn[index6] += this.AnaMagn[index5];
            this.SynFreq[index6] = this.AnaFreq[index5] * pitchShift;
          }
        }
        for (long index7 = 0; index7 <= num1; ++index7)
        {
          double num16 = (double) this.SynMagn[index7];
          double num17 = 2.0 * Math.PI * (((double) this.SynFreq[index7] - (double) index7 * num3) / num3) / (double) osamp + (double) index7 * num4;
          this.SumPhase[index7] += (float) num17;
          double num18 = (double) this.SumPhase[index7];
          this.FfTworksp[2L * index7] = (float) (num16 * Math.Cos(num18));
          this.FfTworksp[2L * index7 + 1L] = (float) (num16 * Math.Sin(num18));
        }
        for (long index8 = fftFrameSize + 2L; index8 < 2L * fftFrameSize; ++index8)
          this.FfTworksp[index8] = 0.0f;
        this.ShortTimeFourierTransform(this.FfTworksp, fftFrameSize, 1L);
        for (long index9 = 0; index9 < fftFrameSize; ++index9)
        {
          double num19 = -0.5 * Math.Cos(2.0 * Math.PI * (double) index9 / (double) fftFrameSize) + 0.5;
          this.OutputAccum[index9] += (float) (2.0 * num19) * this.FfTworksp[2L * index9] / (float) (num1 * osamp);
        }
        for (long index10 = 0; index10 < num2; ++index10)
          this.OutFifo[index10] = this.OutputAccum[index10];
        for (long index11 = 0; index11 < fftFrameSize; ++index11)
          this.OutputAccum[index11] = this.OutputAccum[index11 + num2];
        for (long index12 = 0; index12 < num5; ++index12)
          this.InFifo[index12] = this.InFifo[index12 + num2];
      }
    }
  }

  public void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
  {
    for (long index1 = 2; index1 < 2L * fftFrameSize - 2L; index1 += 2L)
    {
      long num1 = 2;
      long index2 = 0;
      for (; num1 < 2L * fftFrameSize; num1 <<= 1)
      {
        if ((index1 & num1) != 0L)
          ++index2;
        index2 <<= 1;
      }
      if (index1 < index2)
      {
        float num2 = fftBuffer[index1];
        fftBuffer[index1] = fftBuffer[index2];
        fftBuffer[index2] = num2;
        float num3 = fftBuffer[index1 + 1L];
        fftBuffer[index1 + 1L] = fftBuffer[index2 + 1L];
        fftBuffer[index2 + 1L] = num3;
      }
    }
    long num4 = (long) (Math.Log((double) fftFrameSize) / Math.Log(2.0) + 0.5);
    long num5 = 0;
    long num6 = 2;
    for (; num5 < num4; ++num5)
    {
      num6 <<= 1;
      long num7 = num6 >> 1;
      float num8 = 1f;
      float num9 = 0.0f;
      float num10 = 3.14159274f / (float) (num7 >> 1);
      float num11 = (float) Math.Cos((double) num10);
      float num12 = (float) sign * (float) Math.Sin((double) num10);
      for (long index3 = 0; index3 < num7; index3 += 2L)
      {
        for (long index4 = index3; index4 < 2L * fftFrameSize; index4 += num6)
        {
          float num13 = (float) ((double) fftBuffer[index4 + num7] * (double) num8 - (double) fftBuffer[index4 + num7 + 1L] * (double) num9);
          float num14 = (float) ((double) fftBuffer[index4 + num7] * (double) num9 + (double) fftBuffer[index4 + num7 + 1L] * (double) num8);
          fftBuffer[index4 + num7] = fftBuffer[index4] - num13;
          fftBuffer[index4 + num7 + 1L] = fftBuffer[index4 + 1L] - num14;
          fftBuffer[index4] += num13;
          fftBuffer[index4 + 1L] += num14;
        }
        float num15 = (float) ((double) num8 * (double) num11 - (double) num9 * (double) num12);
        num9 = (float) ((double) num8 * (double) num12 + (double) num9 * (double) num11);
        num8 = num15;
      }
    }
  }
}
