using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private float _distanceBeforeSpawn = 100f;
    [SerializeField] private int _initialSegments = 10;
    [SerializeField] private int _initialTransitionSegments = 2;
    [SerializeField] private float _maxSegmentsOnScreen = 15f;
    [SerializeField] Transform _cameraContainer = null;
    [SerializeField] private List<Piece> _ramps = new List<Piece>();
    [SerializeField] private List<Piece> _longBlocks = new List<Piece>();
    [SerializeField] private List<Piece> _jumps = new List<Piece>();
    [SerializeField] private List<Piece> _slides = new List<Piece>();
    [SerializeField] private List<Piece> _pieces = new List<Piece>(); // General Pool
    [SerializeField] private List<Segment> _availableSegments = new List<Segment>();
    [SerializeField] private List<Segment> _availableTransitions = new List<Segment>();
    [SerializeField] private List<Segment> _segments = new List<Segment>();
    [SerializeField] private bool _showCollider = true;

    private int _amountOfActiveSegments;
    private int _continuousSegments;
    private int _currentSpanwZ;
    private int y1, y2, y3;


    public static LevelManager Instance { set; get; }
    public bool ShowCollider { get { return _showCollider; } }
    public ReadOnlyCollection<Piece> Ramps { get { return _ramps.AsReadOnly(); }}
    public ReadOnlyCollection<Piece> LongBlocks { get { return _longBlocks.AsReadOnly(); } }
    public ReadOnlyCollection<Piece> Jumps { get { return _jumps.AsReadOnly(); } }
    public ReadOnlyCollection<Piece> Slides { get { return _slides.AsReadOnly(); } }
    public ReadOnlyCollection<Piece> Pieces { get { return _pieces.AsReadOnly(); } }

    private void Awake()
    {
        Instance = this;
        _cameraContainer = Camera.main.transform;
        _currentSpanwZ = 0;
    }

    private void Start()
    {
        for (int i = 0; i < _initialSegments; i++)
        {
            if (i < _initialTransitionSegments)
            {
                SpawnTransition();
            }
            else
            {
                GenerateSegment();
            }
        }
           
    }

    private void Update()
    {
        if (_currentSpanwZ - _cameraContainer.position.z < _distanceBeforeSpawn)
            GenerateSegment();
          
        if (_amountOfActiveSegments >= _maxSegmentsOnScreen)
        {
            _segments[_amountOfActiveSegments - 1].DeSpawn();
            _amountOfActiveSegments--;
        }
        
    }


    private void GenerateSegment()
    {
        SpawnSegment();
        if(Random.Range(0f,1f)< (_continuousSegments * 0.25f))
        {
            _continuousSegments = 0;
            SpawnTransition();
        }
        else
        {
            _continuousSegments++;
        }
        
    }

    private void SpawnSegment()
    {
        List<Segment> possibleSeg = _availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count);

        Segment s = GetSegment(id, false);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * _currentSpanwZ;
        _currentSpanwZ += s.lenght;
        _amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = _availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * _currentSpanwZ;
        _currentSpanwZ += s.lenght;
        _amountOfActiveSegments++;
        s.Spawn();
    }

    //Get the segment from pool
    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = _segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);
        if(s == null)
        {
            GameObject go = Instantiate((transition) ? _availableTransitions[id].gameObject : _availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();
            s.SegId = id;
            s.transition = transition;
            _segments.Insert(0, s);
        }
        else
        {
            _segments.Remove(s);
            _segments.Insert(0, s);
        }

        return s;
    }

    public Piece GetPiece (PieceType pt, int visualIndex)
    {
        Piece p = _pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject go = null;
            if (pt == PieceType.ramp)
                go = _ramps[visualIndex].gameObject;
            else if (pt == PieceType.longblock)
                go = _longBlocks[visualIndex].gameObject;
            else if (pt == PieceType.jump)
                go = _jumps[visualIndex].gameObject;
            else if (pt == PieceType.slide)
                go = _slides[visualIndex].gameObject;

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            _pieces.Add(p);
        }

        return p;
    }
}
