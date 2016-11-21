var ignoreRootName : String[];
var distanceTolerance : float = 0.4;
var maxOpacity : float = 1.0;

private var opacity : float = 1.0;
private var castingPoint : Transform;
private var buffer : float = 0.02;


function Start(){
	GetComponent.<Renderer>().enabled = true;
	castingPoint = transform.Find("castingPoint");
	castingPoint.parent = transform.parent;
	transform.parent = transform.root;
}

function LateUpdate () {
	//Opacity distance.
	var distanceShadow : float = Vector3.Distance(transform.position, castingPoint.position);
	opacity = Mathf.Lerp(maxOpacity, 0.0,  distanceShadow * (1/distanceTolerance));
	//Shadow position.
	transform.position = castingPoint.position;
	var shadowHits : RaycastHit[];
	shadowHits = Physics.RaycastAll(transform.position + Vector3.up*0.5, -Vector3.up);
	var maxShadowYPosition : float = -999999;
	for (var i = 0; i < shadowHits.Length; i++){
		var shadowHit : RaycastHit = shadowHits[i];
		var name : String = shadowHit.transform.root.name;
		var takeIt : boolean = true;
		for (var n = 0; n < ignoreRootName.Length; n++){
			var ignoreName : String = ignoreRootName[n];
			if(name == ignoreName){
				takeIt = false;
			}
		}
		if(takeIt){
			if(shadowHit.point.y+buffer > maxShadowYPosition){
				maxShadowYPosition = shadowHit.point.y+buffer;
				transform.position.y = shadowHit.point.y+buffer;
				transform.LookAt(transform.position + shadowHit.normal);
			}
		}
	}
	if(shadowHits.Length == 0){
		opacity = 0.0;
	}
	GetComponent.<Renderer>().material.color.a = opacity;
}