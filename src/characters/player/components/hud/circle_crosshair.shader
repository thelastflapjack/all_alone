shader_type canvas_item;

uniform int edgeLength = 600;
uniform int widthPixles = 5;
uniform int innerDiamiter = 300;
uniform vec4 color : hint_color;

void fragment()
{
	vec2 st = UV;
	float width = float(widthPixles) / float(edgeLength);
	float innerBound = (float(innerDiamiter) * 0.5) / float(edgeLength);
	float outerBound = innerBound + width;
	float midBound = outerBound - (0.5 * width);

	float outsideMask = 1.0 - smoothstep(outerBound, midBound, distance(st, vec2(0.5)));
	float insideMask = 1.0 - smoothstep(innerBound, midBound, distance(st, vec2(0.5)));
	float visibleFactor = 1.0 - insideMask - outsideMask;

	COLOR = vec4(color.r, color.g, color.b, color.a * visibleFactor);
}