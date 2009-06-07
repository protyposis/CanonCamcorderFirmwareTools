using System;
using System.Drawing;

namespace LCDDataInterpreter.Filter {
    class AmbienceRemover: IFilter {

        private readonly byte threshold;
        /// <summary>
        /// If content in a pixel row or column has been found, how many additional rows/cols 
        /// shall we check to ensure that the content is "continuous"?
        /// </summary>
        private readonly byte safetyTryCount;

        private enum Side {
            Left,
            Right,
            Top,
            Bottom
        }

        private class BorderDetector {
            private readonly int hitsNeeded;
            private int firstEncounter;
            private int consecutiveEncounters;
            private int position;

            public BorderDetector(int numberOfHitsNeededForDetection) {
                hitsNeeded = numberOfHitsNeededForDetection;
                firstEncounter = -1;
                consecutiveEncounters = 0;
                position = 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>true if hit detected (use HitPosition to get the value)</returns>
            public bool Hit() {
                position++;
                consecutiveEncounters++;

                if(firstEncounter == -1)
                    firstEncounter = position;

                //Debug.WriteLine(String.Format("HIT @ pos {0}, first encounter {1}, consecutive hits {2} (needed {3})", position, firstEncounter, consecutiveEncounters, hitsNeeded));

                if (consecutiveEncounters == hitsNeeded) {
                    //Debug.WriteLine(String.Format("HIT CONFIRMED @ {0}", position));
                    return true;
                }

                return false;
            }

            public void Miss() {
                position++;
                firstEncounter = -1;
                consecutiveEncounters = 0;
                //Debug.WriteLine(String.Format("MISS @ {0}", position));
            }

            public int HitPosition {
                get {
                    if (firstEncounter > -1)
                        return firstEncounter;
                    throw new Exception("no hit yet!");
                }
            }
        }

        public AmbienceRemover() {
            threshold = 64;
            safetyTryCount = 3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grayscaleContentDetectionThreshold">The grayscale color value above which a pixel
        /// is considered as belonging to the searched content</param>
        /// <param name="tryCountForBorderDetection">How many rows need to contain content after content has
        /// been detected in a row to count as real content (the higher the value, the more noise points are ignored)</param>
        public AmbienceRemover(byte grayscaleContentDetectionThreshold, byte tryCountForBorderDetection) {
            threshold = grayscaleContentDetectionThreshold;
            safetyTryCount = tryCountForBorderDetection;
        }

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            var rect = new Rectangle();

            rect.X = findContentFrom(Side.Left, input) - 1;
            rect.Y = findContentFrom(Side.Top, input) - 1;
            rect.Width = findContentFrom(Side.Right, input) - rect.X + 2;
            rect.Height = findContentFrom(Side.Bottom, input) - rect.Y + 2;

            return RegionSelectPictureBox.Copy(input, rect);
        }

        /// <summary>
        /// ugly spaghetti code, but it gets it's job done
        /// rewrite if better performance needed
        /// </summary>
        /// <param name="side"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private int findContentFrom(Side side, Bitmap input) {
            int from = 0, to = 0;
            Color c;
            var detector = new BorderDetector(safetyTryCount);

            if(side == Side.Left) {
                from = 0;
                to = input.Width;

                for (int x = 0; x < to; x++) {
                    bool hit = false;
                    for (int y = 0; y < input.Height; y++) {
                        // SLOOOOOW, use LockBits
                        c = input.GetPixel(x, y);
                        if (c.R > threshold || c.G > threshold || c.B > threshold) {
                            hit = true;
                            break;
                        }
                    }
                    if(hit) {
                        if (detector.Hit()) {
                            return from + detector.HitPosition;
                        }
                    } else {
                        detector.Miss();
                    }
                }
            } else if (side == Side.Right) {
                from = input.Width - 1;
                to = 0;

                for (int x = from; x > to; x--) {
                    bool hit = false;
                    for (int y = 0; y < input.Height; y++) {
                        // SLOOOOOW, use LockBits
                        c = input.GetPixel(x, y);
                        if (c.R > threshold || c.G > threshold || c.B > threshold) {
                            hit = true;
                            break;
                        }
                    }
                    if(hit) {
                        if (detector.Hit()) {
                            return from - detector.HitPosition;
                        }
                    } else {
                        detector.Miss();
                    }
                }
            } else if (side == Side.Top) {
                from = 0;
                to = input.Height;

                for (int y = from; y < to; y++) {
                    bool hit = false;
                    for (int x = 0; x < input.Width; x++) {
                        // SLOOOOOW, use LockBits
                        c = input.GetPixel(x, y);
                        if (c.R > threshold || c.G > threshold || c.B > threshold) {
                            hit = true;
                            break;
                        }
                    }
                    if(hit) {
                        if (detector.Hit()) {
                            return from + detector.HitPosition;
                        }
                    } else {
                        detector.Miss();
                    }
                }
            } else if (side == Side.Bottom) {
                from = input.Height - 1;
                to = 0;

                for (int y = from; y > to; y--) {
                    bool hit = false;
                    for (int x = 0; x < input.Width; x++) {
                        // SLOOOOOW, use LockBits
                        c = input.GetPixel(x, y);
                        if (c.R > threshold || c.G > threshold || c.B > threshold) {
                            hit = true;
                            break;
                        }
                    }
                    if(hit) {
                        if (detector.Hit()) {
                            return from - detector.HitPosition;
                        }
                    } else {
                        detector.Miss();
                    }
                }
            }

            return from;
        }
    }
}
