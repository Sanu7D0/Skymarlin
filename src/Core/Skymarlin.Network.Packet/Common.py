from typing import List, Tuple, Final

namespace: Final = 'urn:Skymarlin'
sky: Final = '{%s}' % namespace

sizeof: Final = {
    'Byte': 1,
    'UInt16': 2,
    'UInt32': 4,
    'UInt64': 8,
    'Single': 4,
    'Double': 8,
    'String': 0
}