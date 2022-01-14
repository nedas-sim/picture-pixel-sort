# picture-pixel-sort

Program reads pixel data from a jpg file, converts data into numbers,
sorts the numbers and puts sorted pixels back to the photo.
Sorting algorithm implemented: merge sort.

Sorted values are being put diagonally.

4 projects in total:

- MergeSortArray - sorts values which are stored in RAM array.
- MergeSortLinkedList - sorts values which are stored in RAM, made custom LinkedList class.
- MergeSortFile - sorts values which are stored in text files, this data structure is based of an array.
- MergeSortFileLinkedList - sorts values which are stored in text files, this data structure is based of the linked list.

Both variations of linked lists swaps data by changing the pointers to other nodes, not by changing the values inside the nodes.

All projects need folders called "nuotraukosInput" and "nuotraukosOutput".

"nuotraukosInput" should have a single jpg file.
"nuotraukosOutput" will have many bmp files with sorted pixels.

Sorting takes a lot of times (especially with values in files), so sorting is done by increments
(first it does 10x10 region in bottom left, then 20x20 and so on).

- 60x60 linked list sorting in a file took more than 1 hour 30 minutes!!!
