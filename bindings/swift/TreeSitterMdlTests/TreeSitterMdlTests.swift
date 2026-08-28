import XCTest
import SwiftTreeSitter
import TreeSitterMdl

final class TreeSitterMdlTests: XCTestCase {
    func testCanLoadGrammar() throws {
        let parser = Parser()
        let language = Language(language: tree_sitter_mdl())
        XCTAssertNoThrow(try parser.setLanguage(language),
                         "Error loading Mantle Data Language grammar")
    }
}
