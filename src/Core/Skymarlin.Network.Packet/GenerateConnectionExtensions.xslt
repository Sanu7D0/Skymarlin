<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="3.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sky="urn:Skymarlin">
    <xsl:param name="namespace" />
    <xsl:output method="text" indent="yes" />
    <xsl:include href="Common.xslt" />
    
    <xsl:template match="sky:Packets">
        <xsl:text>#nullable enable
using System;
using System.Threading;
using Skymarlin.Network;

namespace </xsl:text>
        <xsl:value-of select="$namespace" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$newline" />
        
        <xsl:text>public static class ConnectionExtensions {</xsl:text>
        <xsl:apply-templates select="sky:Packet" mode="extension" />
        <xsl:text>}</xsl:text>
    </xsl:template>
    
    <xsl:template match="sky:Packet" mode="extension">
        <xsl:value-of select="$newline" />
        <xsl:text>
    public static async ValueTask Send</xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>Async(this IConnection? connection</xsl:text>
        <xsl:if test="count(sky:Fields/sky:Field) > 0">
            <xsl:text>, </xsl:text>
        </xsl:if>
        <xsl:apply-templates select="sky:Fields/sky:Field" mode="params" />
        <xsl:text>)</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text> {
        if (connection is null)
            return;
        
        int WritePacket()
        {
            var length = </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>.Length;</xsl:text>
        
        <xsl:text>;
            var packet = new </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>(connection.Output.GetSpan(length)[..length]);</xsl:text>
        <xsl:if test="sky:Fields/sky:Field">
            <xsl:value-of select="$newline" />
            <xsl:apply-templates select="sky:Fields/sky:Field" mode="assignment" />
        </xsl:if>
        <xsl:text>
            return packet.Header.Length;
        }
        
        await connection.SendAsync(WritePacket).ConfigureWait(false);
    }</xsl:text>
    </xsl:template>
    
    <xsl:template match="sky:Field" mode="length">
        <xsl:call-template name="LowerCaseName" />
    </xsl:template>
    
    <xsl:template match="sky:Field" mode="params">
        <xsl:if test="position() > 1">
            <xsl:text>, </xsl:text>
        </xsl:if>
        <xsl:apply-templates select="sky:Type" mode="type" />
        <xsl:text>@</xsl:text>
        <xsl:call-template name="LowerCaseName" />
        <xsl:if test="sky:DefaultValue">
            <xsl:text> = </xsl:text>
            <xsl:value-of select="sky:DefaultValue" />
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="sky:Field" mode="listParams">
        <xsl:if test="position() > 1">
            <xsl:text>, </xsl:text>
        </xsl:if>
        <xsl:text>@</xsl:text>
        <xsl:call-template name="LowerCaseName" />
    </xsl:template>
    
    <xsl:template match="sky:Field" mode="assignment">
        <xsl:text>            packet.</xsl:text>
        <xsl:value-of select="sky:Name" />
        <xsl:text> = @</xsl:text>
        <xsl:call-template name="LowerCaseName" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
<!--    <xsl:template match="text()" mode="params" />-->
<!--    <xsl:template match="text()" mode="listParams" />-->
<!--    <xsl:template match="text()" mode="assignment" />-->
<!--    <xsl:template match="text()" mode="extension" />-->
<!--    <xsl:template match="text()" mode="length" />-->
</xsl:stylesheet>