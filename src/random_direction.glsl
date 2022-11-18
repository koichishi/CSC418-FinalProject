// Generate a pseudorandom unit 3D vector
// 
// Inputs:
//   seed  3D seed
// Returns psuedorandom, unit 3D vector drawn from uniform distribution over
// the unit sphere (assuming random2 is uniform over [0,1]²).
//
// expects: random2.glsl, PI.glsl
vec3 random_direction( vec3 seed)
{
  vec2 randNum = random2(seed);
  float theta = randNum.x * M_PI * 2;
  float phi   = randNum.y * M_PI;
  return vec3(sin(phi) * cos(theta), sin(theta) * sin(phi), cos(phi));
}
