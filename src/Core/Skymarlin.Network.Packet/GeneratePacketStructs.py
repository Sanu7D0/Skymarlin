#!/usr/bin/env python3
import sys
import time
# from lxml import etree
import xml.etree.ElementTree as etree
from typing import List, Tuple, Final
from Common import *

length_type_map: Final = {
    'A0': 'byte',
    'A1': 'ushort',
    'E0': 'byte',
    'E1': 'ushort'
}


def main():
    if len(sys.argv) != 4:
        raise SystemExit(f'Usage: {sys.argv[0]} [input xml path] [output file path] [target namespace]')
    
    in_path = sys.argv[1]
    out_path = sys.argv[2]
    target_namespace = sys.argv[3]
    print(f'Generate packet structs [{in_path}] --> [{out_path}]... ', end='')
    
    parser = etree.XMLParser()
    packets = etree.parse(in_path, parser).getroot()
    
    result = generate(packets, target_namespace)
    
    with open(out_path, 'w') as out:
        out.write(result)
        
    print('Done')
    

def generate(packets, target_namespace: str) -> str:
    structs = '\n'.join([generate_struct(packet) for packet in packets.iter(sky + 'Packet')])
    
    result = f'''using System;
using Skymarlin.Network.Packet;

namespace {target_namespace};
{structs}'''
    
    return result


def generate_struct(packet) -> str:
    name = packet.find(sky + 'Name').text
    header_type = packet.find(sky + 'HeaderType').text
    code = packet.find(sky + 'Code').text
    
    length_type = length_type_map[header_type]
    
    def generate_properties() -> str:
        fields = packet.find(sky + 'Fields')
        properties: List[str] = []
        index = 0
        for field in fields.iter(sky + 'Field'):
            property, index = generate_property(field, index)
            properties.append(property)
        
        return'\n'.join(properties)
    
    struct = f'''
public readonly struct {name}
{{
    private readonly Memory<byte> _data;
    
    public {name}(Memory<byte> data)
       : (this, true)
    {{
    }}
    
    private {name}(Memory<byte> data, bool initialize)
    {{
        _data = data;
        if (initialize)
        {{
            var header = Header;
            header.Type = HeaderType;
            header.Code = Code;
            header.Length = ({length_type})data.Length;
        }}
    }}
    
    public static byte HeaderType => 0x{header_type};
    
    public static byte Code => 0x{code};
    
    public static {header_type}Header Header => new (_data);
    {generate_properties()}
    
    public static implicit operator {name}(Memory<byte> packet) => new (packet, false);
    
    public static implicit operator Memory<byte>({name} packet) => packet._data;
}}'''
    
    return struct
        
        
def generate_property(field, index: int) -> Tuple[str, int]:
    name = field.find(sky + 'Name').text
    Type = field.find(sky + 'Type').text
    
    span_slice_get = f'[{index}]' if Type == 'byte' else f'[{index}..].Read{Type}()'
    span_slice_set = f'[{index}] = value' if Type == 'byte' else f'[{index}..].Write{Type}(value)'
    
    property = f'''
    public {Type} {name}
    {{
        get => _data.Span{span_slice_get};
        set => _data.Span{span_slice_set};
    }}'''
    
    return property, index + sizeof[Type]
    
    
if __name__ == '__main__':
    main()