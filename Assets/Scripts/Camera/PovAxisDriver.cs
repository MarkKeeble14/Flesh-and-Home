using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera)), DisallowMultipleComponent]
public class PovAxisDriver : MonoBehaviour
{
    public CinemachineInputAxisDriver xAxis;
    public CinemachineInputAxisDriver yAxis;

    private CinemachinePOV pov;

    private void OnEnable()
    {
        var vcam = GetComponent<CinemachineVirtualCamera>();
        pov = vcam == null ? null : vcam.GetCinemachineComponent<CinemachinePOV>();
        pov.m_HorizontalAxis.m_MaxSpeed = pov.m_HorizontalAxis.m_AccelTime = pov.m_HorizontalAxis.m_DecelTime = 0;
        pov.m_HorizontalAxis.m_InputAxisName = string.Empty;
        pov.m_VerticalAxis.m_MaxSpeed = pov.m_VerticalAxis.m_AccelTime = pov.m_VerticalAxis.m_DecelTime = 0;
        pov.m_VerticalAxis.m_InputAxisName = string.Empty;
    }

    private void OnValidate()
    {
        xAxis.Validate();
        yAxis.Validate();
    }

    private void Reset()
    {
        xAxis = new CinemachineInputAxisDriver
        {
            multiplier = 10f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse X",
        };
        yAxis = new CinemachineInputAxisDriver
        {
            multiplier = -10f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse Y",
        };
    }

    private void Update()
    {
        if (pov == null)
            return;
        bool changed = xAxis.Update(Time.deltaTime, ref pov.m_HorizontalAxis);
        changed |= yAxis.Update(Time.deltaTime, ref pov.m_VerticalAxis);
        if (changed)
        {
            pov.m_HorizontalRecentering.CancelRecentering();
            pov.m_VerticalRecentering.CancelRecentering();
        }
    }
}
