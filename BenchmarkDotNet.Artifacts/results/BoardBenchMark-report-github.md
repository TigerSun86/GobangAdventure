``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-5600U CPU 2.60GHz, ProcessorCount=4
Frequency=2533197 Hz, Resolution=394.7581 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2046.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2046.0


```
 |                    Method | Count |           Mean |      StdErr |      StdDev |         Median |      Gen 0 | Allocated |
 |-------------------------- |------ |--------------- |------------ |------------ |--------------- |----------- |---------- |
 | **GetEmptyByPositionManager** |     **1** |      **5.8365 us** |   **0.0584 us** |   **0.3915 us** |      **5.6180 us** |     **1.2710** |   **3.39 kB** |
 |         GetEmptyByForLoop |     1 |      1.9614 us |   0.0281 us |   0.1089 us |      1.9119 us |     0.8657 |   2.02 kB |
 |        GetEmptyByForLoop2 |     1 |      4.4396 us |   0.0466 us |   0.2378 us |      4.3262 us |     1.3051 |   3.11 kB |
 | **GetEmptyByPositionManager** |   **100** |    **575.3503 us** |   **5.7167 us** |  **44.6487 us** |    **554.8586 us** |   **151.3672** | **334.85 kB** |
 |         GetEmptyByForLoop |   100 |    197.8725 us |   1.4641 us |   5.6705 us |    198.3117 us |    85.7077 | 197.64 kB |
 |        GetEmptyByForLoop2 |   100 |    102.8321 us |   0.8484 us |   3.2859 us |    101.5723 us |          - |   3.11 kB |
 | **GetEmptyByPositionManager** | **10000** | **55,196.5931 us** | **229.8428 us** | **859.9932 us** | **55,210.3462 us** | **15387.5000** |  **33.48 MB** |
 |         GetEmptyByForLoop | 10000 | 19,768.7485 us | 211.2697 us | 845.0787 us | 19,417.3521 us |  9150.0000 |  19.76 MB |
 |        GetEmptyByForLoop2 | 10000 |  9,832.2538 us |  88.1599 us | 329.8643 us |  9,697.7154 us |          - |   3.33 kB |
