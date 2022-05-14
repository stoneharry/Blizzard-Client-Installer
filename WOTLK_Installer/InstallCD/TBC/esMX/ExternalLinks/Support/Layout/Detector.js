function detection() {
	var system = navigator.platform;
	var vString = navigator.appVersion;
	var version1 = vString.match( "MSIE 5.0" );
	var version2 = vString.match( "MSIE 5.5" );
	
	if( version1 != null & system != "MacPPC")
		document.body.style.overflow = 'scroll';
	
	if( version2 != null & system != "MacPPC")
		document.body.style.overflow = 'scroll';
} 
