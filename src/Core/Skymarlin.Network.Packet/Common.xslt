<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="3.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:sky="urn:Skymarlin">
    <xsl:variable name="newline">
        <xsl:text>&#xA;</xsl:text>
    </xsl:variable>
    <xsl:variable name="upperCaseLetters">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>
    <xsl:variable name="lowerCaseLetters">abcdefghijklmnopqrstuvwxyz</xsl:variable>
    <xsl:variable name="digits">0123456789</xsl:variable>
    
    <xsl:template name="LowerCaseName">
        <xsl:value-of select="concat(translate(substring(sky:Name, 1, 1), $upperCaseLetters, $lowerCaseLetters), substring(sky:Name, 2))" />
    </xsl:template>
    
    <!-- Type Mapping -->
    <xsl:template match="sky:Type[. = 'Boolean']" mode="type">bool</xsl:template>
    <xsl:template match="sky:Type[. = 'Byte']" mode="type">byte</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt16']" mode="type">ushort</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt32']" mode="type">uint</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt64']" mode="type">ulong</xsl:template>
    <xsl:template match="sky:Type[. = 'Single']" mode="type">float</xsl:template>
    <xsl:template match="sky:Type[. = 'Double']" mode="type">double</xsl:template>
    <xsl:template match="sky:Type[. = 'String']" mode="type">string</xsl:template>
    <xsl:template match="text()" mode="type" />
    
    <!-- Type Sizeof -->
    <xsl:function name="sky:sizeof" as="xs:int">
        <xsl:param name="type" as="sky:Type" />
    </xsl:function>
    <xsl:template match="sky:Type[. = 'Boolean']" mode="sizeof">1</xsl:template>
    <xsl:template match="sky:Type[. = 'Byte']" mode="sizeof">1</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt16']" mode="sizeof">2</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt32']" mode="sizeof">4</xsl:template>
    <xsl:template match="sky:Type[. = 'UInt64']" mode="sizeof">8</xsl:template>
    <xsl:template match="sky:Type[. = 'Single']" mode="sizeof">4</xsl:template>
    <xsl:template match="sky:Type[. = 'Double']" mode="sizeof">8</xsl:template>
    <xsl:template match="text()" mode="sizeof" />
</xsl:stylesheet>