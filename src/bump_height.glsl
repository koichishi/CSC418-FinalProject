// Create a bumpy surface by using procedural noise to generate a height (
// displacement in normal direction).
//
// Inputs:
//   is_moon  whether we're looking at the moon or centre planet
//   s  3D position of seed for noise generation
// Returns elevation adjust along normal (values between -0.1 and 0.1 are
//   reasonable.
float bump_height(bool is_moon, vec3 s)
{
  float noise;
  if (is_moon) {
    noise = pow(acos(improved_perlin_noise(7 * s)), 2);
    return smooth_heaviside(noise, 1);
  }
  noise = improved_perlin_noise(vec3(5) * improved_perlin_noise(s));
  return smooth_heaviside(noise, 10) / 10.0;
}

