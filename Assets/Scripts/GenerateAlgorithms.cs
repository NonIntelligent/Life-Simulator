using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAlgorithms
{
    static void SquareStep(float[,] arr, int x, int z, int reach, int maxSize, float randMax) {
		int count = 0;
		float avg = 0f;
		// Check if the point to sample is within bounds of the array

		if (x - reach >= 0) {
			avg += arr[x - reach, z];
			count++;
		}

		if (x + reach < maxSize) {
			avg += arr[x + reach, z];
			count++;
		}

		if (z - reach >= 0) {
			avg += arr[x, z - reach];
			count++;
		}

		if (z + reach < maxSize) {
			avg += arr[x, z + reach];
			count++;
		}

		avg /= count;
		avg += Random.Range(-randMax, randMax);
		arr[x, z] = avg;
	}

	static void DiamondStep(float[,] arr, int x, int z, int reach, int maxSize, float randMax) {
		float avg = 0f;
		// Check if the point to sample is within bounds of the array

		avg += arr[x - reach, z - reach];

		avg += arr[x - reach, z + reach];

		avg += arr[x + reach, z - reach];

		avg += arr[x + reach, z + reach];

		avg /= 4;
		avg += Random.Range(-randMax, randMax);
		arr[x, z] = avg;
	}

	public static void DiamondSquare(float[,] arr, int size, int maxSize, float randMax, float h) {

		int midpoint = size / 2;
		if (midpoint < 1) return;

		for (int x = midpoint; x < maxSize; x += size) {
			for (int z = midpoint; z < maxSize; z += size) {
				DiamondStep(arr, x % maxSize, z % maxSize, midpoint, maxSize, randMax);
			}
		}

		int currentColumn = 0;

		for (int x = 0; x < maxSize; x += midpoint) {
			currentColumn++;

			// Odd column
			if (currentColumn % 2 == 1) {
				for (int z = midpoint; z < maxSize; z += size) {
					SquareStep(arr, x % maxSize, z % maxSize, midpoint, maxSize, randMax);
				}
			}
			else {
				for (int z = 0; z <= maxSize; z += size) {
					SquareStep(arr, x % maxSize, z % maxSize, midpoint, maxSize, randMax);
				}
			}
		}

		DiamondSquare(arr, midpoint, maxSize, randMax * Mathf.Pow(2, -h), h);
	}

	public static void PerlinNoise(float[,] arr, int maxSize, int octaves, float h, float minHeight, float maxHeight) {
		float amplitude = 1f;
		float scalar = 1f;
		float divideSum = 0f;
		float result = 0f, outputHeight = 0f;
		float delta = maxHeight - minHeight;

		for (int z = 0; z < maxSize; z++) {
			for (int x = 0; x < maxSize; x++) {
				float nx = x / maxSize - 0.5f, nz = z / maxSize - 0.5f;

				amplitude = 1f;
				scalar = 1f;
				divideSum = 0f;
				result = 0f;
				outputHeight = 0f;

				// Combine multiple frequencies
				for (int i = 0; i < octaves; i++) {
					result += amplitude * Mathf.PerlinNoise(scalar * nx, scalar * nz);
					divideSum += amplitude;
					amplitude *= 0.5f;
					scalar *= 2f;
				}

				// Average the noise values
				result /= divideSum;

				// flatten or stretch valleys
				if (result <= 0.0) {
					result = Mathf.Pow(result, (int)h);
				}
				else {
					result = Mathf.Pow(result, h);
				}

				outputHeight = delta * result + minHeight;

				arr[x, z] = outputHeight;
			}
		}
	}
			

	public static void SetRandomSeed(int seed) {
		Random.InitState(seed);
    }
}
