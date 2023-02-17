<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sky="urn:Skymarlin">
    <xsl:variable name="newline">
        <xsl:text xml:space="preserve" />
    </xsl:variable>
    
    <!-- Type Mapping   -->
    <xsl:template match="sky:Type[. = 'Bool']" mode="type">bool</xsl:template>
    <xsl:template match="sky:Type[. = 'Byte']" mode="type">byte</xsl:template>
    <xsl:template match="sky:Type[. = 'Short']" mode="type">ushort</xsl:template>
    <xsl:template match="sky:Type[. = 'Int']" mode="type">uint</xsl:template>
    <xsl:template match="sky:Type[. = 'Long']" mode="type">ulong</xsl:template>
    <xsl:template match="sky:Type[. = 'Float']" mode="type">float</xsl:template>
    <xsl:template match="sky:Type[. = 'Double']" mode="type">double</xsl:template>
    <xsl:template match="sky:Type[. = 'String']" mode="type">string</xsl:template>
</xsl:stylesheet>