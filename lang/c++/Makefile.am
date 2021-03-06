
BOOST_CPPFLAGS = @BOOST_CPPFLAGS@
BOOST_LDFLAGS = @BOOST_LDFLAGS@
BOOST_REGEX_LIB = @BOOST_REGEX_LIB@
PYTHON = @PYTHON@

AM_CXXFLAGS= -I$(top_srcdir)/api $(BOOST_CPPFLAGS) -Wall -Wno-invalid-offsetof
# removed -Werror for Cygwin builds, maybe should add a check flag

ACLOCAL_AMFLAGS=-I m4

library_includedir=$(includedir)/avrocpp
library_include_HEADERS = \
api/AvroParse.hh \
api/AvroSerialize.hh \
api/AvroTraits.hh \
api/Boost.hh \
api/Compiler.hh \
api/CompilerNode.hh \
api/Exception.hh \
api/Layout.hh \
api/Node.hh \
api/NodeConcepts.hh \
api/NodeImpl.hh \
api/Parser.hh \
api/Reader.hh \
api/Resolver.hh \
api/ResolverSchema.hh \
api/ResolvingReader.hh \
api/Schema.hh \
api/SchemaResolution.hh \
api/Serializer.hh \
api/SymbolMap.hh \
api/Types.hh \
api/ValidSchema.hh \
api/Validator.hh \
api/Writer.hh \
api/Zigzag.hh \
api/buffer/BufferStreambuf.hh \
api/buffer/detail/BufferDetailIterator.hh \
api/buffer/detail/BufferDetail.hh \
api/buffer/BufferStream.hh \
api/buffer/BufferPrint.hh \
api/buffer/Buffer.hh \
api/buffer/BufferReader.hh

BUILT_SOURCES = AvroYacc.h testgen.hh testgen2.hh

bin_PROGRAMS = precompile testparser 
bin_SCRIPTS = scripts/gen-cppcode.py

precompile_SOURCES = test/precompile.cc

precompile_LDFLAGS = -static $(BOOST_LDFLAGS)
precompile_LDADD = $(top_builddir)/libavrocpp.la $(BOOST_REGEX_LIB)

testparser_SOURCES = test/testparser.cc

testparser_LDFLAGS = -static -no-install $(BOOST_LDFLAGS)
testparser_LDADD = $(top_builddir)/libavrocpp.la $(BOOST_REGEX_LIB)

lib_LTLIBRARIES = libavrocpp.la

libavrocpp_la_SOURCES = $(library_include_HEADERS) \
api/Zigzag.hh \
impl/Compiler.cc \
impl/CompilerNode.cc \
impl/Node.cc \
impl/NodeImpl.cc \
impl/Resolver.cc \
impl/ResolverSchema.cc \
impl/Schema.cc \
impl/Types.cc \
impl/ValidSchema.cc \
impl/Validator.cc \
impl/Zigzag.cc \
parser/AvroYacc.yy \
parser/AvroLex.ll 

# libavrocpp_la_LDFLAGS = -export-dynamic

AM_LFLAGS= -o$(LEX_OUTPUT_ROOT).c
AM_YFLAGS = -d

check_PROGRAMS = unittest testgen buffertest

TESTS=unittest testgen buffertest
TESTS_ENVIRONMENT = top_srcdir=$(top_srcdir)

unittest_SOURCES = test/unittest.cc
unittest_LDFLAGS = -static -no-install $(BOOST_LDFLAGS)
unittest_LDADD = $(top_builddir)/libavrocpp.la $(BOOST_REGEX_LIB)

testgen_SOURCES = test/testgen.cc testgen.hh testgen2.hh
testgen_LDFLAGS = -static -no-install $(BOOST_LDFLAGS)
testgen_LDADD = $(top_builddir)/libavrocpp.la $(BOOST_REGEX_LIB)

buffertest_SOURCES = test/buffertest.cc 
buffertest_LDFLAGS = -static -no-install $(BOOST_LDFLAGS)
buffertest_LDADD = $(top_builddir)/libavrocpp.la $(BOOST_REGEX_LIB) $(BOOST_THREAD_LIB) $(BOOST_SYSTEM_LIB) $(BOOST_FILESYSTEM_LIB)

# Make sure we never package up '.svn' directories
dist-hook:
	find $(distdir) -name '.svn' | xargs rm -rf

testgen.hh : bigrecord.precompile $(top_srcdir)/scripts/gen-cppcode.py
	$(PYTHON) $(top_srcdir)/scripts/gen-cppcode.py -n testgen -i $< -o $@

testgen2.hh : bigrecord2.precompile $(top_srcdir)/scripts/gen-cppcode.py 
	$(PYTHON) $(top_srcdir)/scripts/gen-cppcode.py -n testgen2 -i $< -o $@

bigrecord.precompile: $(top_srcdir)/jsonschemas/bigrecord precompile$(EXEEXT)
	$(top_builddir)/precompile$(EXEEXT) < $< > $@

bigrecord2.precompile: $(top_srcdir)/jsonschemas/bigrecord2 precompile$(EXEEXT) 
	$(top_builddir)/precompile$(EXEEXT) < $< > $@

DOXYGEN_INPUTS= $(top_srcdir)/MainPage.dox $(patsubst %,$(top_srcdir)/%, $(library_include_HEADERS))

CPP_DOC_DIR ?= "$(top_builddir)/doc"

doc: $(CPP_DOC_DIR)/html/index.html

$(CPP_DOC_DIR)/html/index.html: $(DOXYGEN_INPUTS) $(srcdir)/Doxyfile
	(cat $(srcdir)/Doxyfile; echo "OUTPUT_DIRECTORY=$(CPP_DOC_DIR)";  echo "INPUT=$(DOXYGEN_INPUTS)") | $(DOXYGEN) - ;

EXTRA_DIST=jsonschemas scripts

CLEANFILES=bigrecord.precompile bigrecord2.precompile testgen.hh testgen2.hh AvroLex.cc AvroYacc.cc AvroYacc.h test.avro

clean-local: clean-local-check
.PHONY: clean-local-check
clean-local-check:
	-rm -rf $(CPP_DOC_DIR)

