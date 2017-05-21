using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    private KeywordRecognizer _keywordRecognizer;
    private readonly Dictionary<string, System.Action> _keywords = new Dictionary<string, System.Action>();
    private readonly System.Random _random = new System.Random();
    private GameObject _container;
    private GameObject _firstCube;

    // Use this for initialization
    void Start()
    {
        _container = GameObject.Find("Container");
        _firstCube = GameObject.Find("Cube");

        _keywords.Add("Add cube", AddCube);
        _keywords.Add("Reset scene", ResetScene);

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        if (_keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void ResetScene()
    {
        foreach (Transform t in _container.transform)
        {
            var cube = t.gameObject;
            Destroy(cube);
        }
    }

    private void AddCube()
    {
        var container = GameObject.Find("Container");

        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.SetParent(container.transform, true);
        newCube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        newCube.transform.rotation = Quaternion.Euler(
            45f,
            0,
            45f);

        newCube.transform.localPosition = new Vector3(0f, 0f, 0f);

        var rigidBody = newCube.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;

        var renderer = _firstCube.GetComponent<Renderer>();

        var material = new Material(renderer.material.shader);

        var color = new Color(
            (float)_random.NextDouble(),
            (float)_random.NextDouble(),
            (float)_random.NextDouble());

        material.SetColor("_Color", color);

        var newRenderer = newCube.GetComponent<Renderer>();
        newRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
    }
}