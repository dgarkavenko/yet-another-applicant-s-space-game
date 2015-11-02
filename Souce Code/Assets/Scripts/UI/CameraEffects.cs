/**
This work is licensed under a Creative Commons Attribution 3.0 Unported License.
http://creativecommons.org/licenses/by/3.0/deed.en_GB

You are free:

to copy, distribute, display, and perform the work
to make derivative works
to make commercial use of the work
*/

using UnityEngine;

public class CameraEffects : MonoBehaviour {


    private float _relativeSpeed;

    public float RelativeSpeed {
        get { return _relativeSpeed; }
        set {

            _relativeSpeed = value;
            Stars.SetTextureScale("_Stars", new Vector2(1, Mathf.Lerp(1, _closeTiling, _relativeSpeed)));
            Stars.SetTextureScale("_StarsFar", new Vector2(1, Mathf.Lerp(1, _farTiling, _relativeSpeed)));
        }
    }

    public float JAMMERIntensity;
    public float HITIntensity;


    private float _closeTiling = 0.02f;
    private float _farTiling = 0.05f;
    private float _closeSpeed = 0.5f;
    private float _farSpeed = 0.2f;

	public Texture2D displacementMap;
	float glitchup, glitchdown, flicker,
			glitchupTime = 0.05f, glitchdownTime = 0.05f, flickerTime = 0.5f;
	
	private float _intensity;
    public float Freq = 1;
    public float Distort = 10;


    public Material GlitchMaterial;
    public Material TVMaterial;
    public Material Stars;

    void Start()
    {
        RelativeSpeed = 0;
        GlitchMaterial.SetFloat("filterRadius", 1);
        GlitchMaterial.SetFloat("flip_up", 0);
        GlitchMaterial.SetFloat("flip_down", 1);
    }

    public float CloseSpeed;
    public float FarSpeed;


    void Update()
    {
        Stars.SetTextureOffset("_Stars", new Vector2(0, Time.time * _closeSpeed * RelativeSpeed));
        Stars.SetTextureOffset("_StarsFar", new Vector2(0, Time.time * _farSpeed * RelativeSpeed));

        if(HITIntensity > 0)
        {
            HITIntensity -= 0.2f;
            HITIntensity = Mathf.Max(0, HITIntensity);
        }

    }


    void OnRenderImage (RenderTexture source, RenderTexture destination) {

        _intensity = JAMMERIntensity + HITIntensity;

        GlitchMaterial.SetFloat("_Intensity", _intensity);
		GlitchMaterial.SetTexture("_DispTex", displacementMap);
        TVMaterial.SetFloat("_Distort", Distort);
		
		glitchup += Time.deltaTime * Freq;
		glitchdown += Time.deltaTime * Freq;
		flicker += Time.deltaTime * Freq;
		
		if(flicker > flickerTime){
			GlitchMaterial.SetFloat("filterRadius", Random.Range(0f, 3f) * Mathf.Max(1, _intensity));
			flicker = 0;
			flickerTime = Random.value;
		}
		
		if(glitchup > glitchupTime){
			if(Random.value < 0.1f * _intensity)
				GlitchMaterial.SetFloat("flip_up", Random.Range(-1, 3f) * _intensity);
			else
				GlitchMaterial.SetFloat("flip_up", 0);
			
			glitchup = 0;
			glitchupTime = Random.value/10f;
		}
		
		if(glitchdown > glitchdownTime){
			if(Random.value < 0.1f * _intensity)
				GlitchMaterial.SetFloat("flip_down", 1-Random.Range(0, 1f) * _intensity);
			else
				GlitchMaterial.SetFloat("flip_down", 1);
			
			glitchdown = 0;
			glitchdownTime = Random.value/10f;
		}
		
		if(Random.value < 0.05 * _intensity){
			GlitchMaterial.SetFloat("displace", Random.value * _intensity);
			GlitchMaterial.SetFloat("scale", 1-Random.value * _intensity);
		}else
			GlitchMaterial.SetFloat("displace", 0);

        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height);

        Graphics.Blit (source, rt, GlitchMaterial); // This one isn't my shader.
        Graphics.Blit(rt, destination, TVMaterial);

        RenderTexture.ReleaseTemporary(rt);

    }
}