// Set the pixel color to blue or gray depending on is_moon.
//
// Uniforms:
uniform bool is_moon;
uniform bool is_boom;
// Outputs:
out vec3 color;
void main()
{
  if (is_moon) {
      color = vec3(0.5, 0.5, 0.5); 
  }
  else if (!is_boom) {
      color = vec3(0.0, 0.0, 1.0);
  }
  else {
      color = vec3(1., 0., 0.);
  }
}
