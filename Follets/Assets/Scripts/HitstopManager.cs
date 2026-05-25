using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    private bool frozen = false;

    void Awake()
    {

    }

    /// <summary>
    /// Congela el tiempo del juego de forma variable.
    /// </summary>
    /// <param name="duracion">Cuánto tiempo durará el parón (en segundos reales).</param>
    /// <param name="escalaTiempo">0 para congelación total, o un valor bajo (ej. 0.1) para cámara lenta.</param>
    public void ActivarHitstop(float duracion, float escalaTiempo = 0f)
    {
        // Si ya hay un hitstop activo, no acumulamos corrutinas problemáticas
        if (frozen) return;

        StartCoroutine(RutinaHitstop(duracion, escalaTiempo));
    }

    private IEnumerator RutinaHitstop(float duracion, float escalaTiempo)
    {
        frozen = true;

        // Guardamos el fixedDeltaTime original para no romper las físicas al volver
        float fixedDeltaTimeOriginal = Time.fixedDeltaTime;

        // Aplicamos el stop / ralentización
        Time.timeScale = escalaTiempo;

        // Ajustamos las físicas para que vayan acorde al nuevo tiempo
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        // IMPORTANTE: Usamos WaitForSecondsRealtime porque el WaitForSeconds normal 
        // se vería afectado por el propio timeScale y se congelaría para siempre.
        yield return new WaitForSecondsRealtime(duracion);

        // Restauramos los valores originales
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTimeOriginal;

        frozen = false;
    }
}